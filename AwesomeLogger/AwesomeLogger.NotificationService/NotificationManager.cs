using System;
using System.Diagnostics;
using System.Net.Mail;
using System.Threading.Tasks;
using AwesomeLogger.NotificationService.Configuration;
using AwesomeLogger.NotificationService.Exceptions;
using AwesomeLogger.NotificationService.Models;
using AwesomeLogger.NotificationService.Services;
using Microsoft.ServiceBus.Messaging;

namespace AwesomeLogger.NotificationService
{
    internal class NotificationManager : INotificationManager
    {
        private readonly IAuditService _auditService;
        private readonly IConfigurationProvider _config;
        private readonly IEmailService _emailService;
        private SubscriptionClient _client;

        public NotificationManager(IEmailService emailService, IConfigurationProvider config, IAuditService auditService)
        {
            _emailService = emailService;
            _config = config;
            _auditService = auditService;
        }

        public void Start()
        {
            // Process messages
            _client =
                SubscriptionClient.CreateFromConnectionString(_config.Get(SettingNames.ServiceBusConnectionString),
                    _config.Get(SettingNames.ServiceBusNotifyTopic),
                    _config.Get(SettingNames.ServiceBusNotifyChannel));

            _client.OnMessage(message =>
            {
                try
                {
                    object machineName;
                    message.Properties.TryGetValue("MachineName", out machineName);

                    object searchPath;
                    message.Properties.TryGetValue("SearchPath", out searchPath);

                    object logPath;
                    message.Properties.TryGetValue("Path", out logPath);

                    object pattern;
                    message.Properties.TryGetValue("Pattern", out pattern);

                    object matchLine;
                    message.Properties.TryGetValue("Match", out matchLine);

                    object lineNumber;
                    message.Properties.TryGetValue("Line", out lineNumber);

                    object email;
                    message.Properties.TryGetValue("Email", out email);

                    var dateTime = message.EnqueuedTimeUtc;

                    // Audit
                    var match = new PatternMatchModel
                    {
                        MachineName = string.Format("{0}", machineName),
                        SearchPath =  string.Format("{0}", searchPath),
                        LogPath = string.Format("{0}", logPath),
                        Pattern = string.Format("{0}", pattern),
                        Line = lineNumber != null ? int.Parse(lineNumber.ToString()) : -1,
                        Email = string.Format("{0}", email),
                        Match = string.Format("{0}", matchLine),
                        Created = dateTime
                    };

                    // fixing missing fields
                    if (string.IsNullOrEmpty(match.SearchPath))
                    {
                        match.SearchPath = match.LogPath;
                    }

                    if (!AuditAsync(match).Result)
                    {
                        // conflict detected
                        message.Complete();
                        return;
                    }

                    // Log
                    Log(match);

                    // Send email
                    SendEmailAsync(match).Wait();

                    message.Complete();
                }
                catch (Exception e)
                {
                    // Could not process message
                    message.Abandon();

                    var msg = string.Format("Failed to process message: {0}", e);
                    Trace.TraceError(msg);
                }
            });
        }

        public void Dispose()
        {
            _client.Close();
        }

        private async Task<bool> AuditAsync(PatternMatchModel match)
        {
            try
            {
                await _auditService.AddAsync(match);
            }
            catch (ConflictException)
            {
                return false;
            }

            return true;
        }

        private async Task SendEmailAsync(PatternMatchModel match)
        {
            if (string.IsNullOrEmpty(match.Email))
            {
                return;
            }

            var notificationAddress = _config.Get(SettingNames.NotificationAddress);
            var subject = string.Format("AwesomeLogger match found on machine {0}", match.MachineName);
            var htmlBody = string.Format("Machine: {0}<br /><br />" +
                                         "Search Path: {1}<br /><br />" +
                                         "Log Path: {2}<br /><br />" +
                                         "Pattern: {3}<br /><br />" +
                                         "Line number: {4}<br /><br />" +
                                         "Match: {5}<br /><br />" +
                                         "Date and Time: {6}",
                match.MachineName, match.SearchPath, match.LogPath, match.Pattern, match.Line, match.Match,
                match.Created);
            var msg = new MailMessage
            {
                Subject = subject,
                IsBodyHtml = true,
                Body = htmlBody,
                From = new MailAddress(notificationAddress),
                To = {match.Email}
            };

            await _emailService.SendAsync(msg);
        }

        private static void Log(PatternMatchModel match)
        {
            var body = string.Format("Pattern match found.\n\n" +
                                     "Machine: {0}\n" +
                                     "Search Path: {1}\n" +
                                     "Log Path: {2}\n" +
                                     "Pattern: {3}\n" +
                                     "Line number: {4}\n" +
                                     "Match: {5}\n" +
                                     "Email: {6}\n" +
                                     "Date and Time: {7}",
                match.MachineName, match.SearchPath, match.LogPath, match.Pattern, match.Line, match.Match, match.Email,
                match.Created);
            Trace.TraceInformation(body);
        }
    }
}