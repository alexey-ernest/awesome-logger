using System;
using System.Diagnostics;
using System.Net.Mail;
using AwesomeLogger.NotificationService.Configuration;
using AwesomeLogger.NotificationService.Services;
using Microsoft.ServiceBus.Messaging;

namespace AwesomeLogger.NotificationService
{
    internal class NotificationManager : INotificationManager
    {
        private readonly IConfigurationProvider _config;
        private readonly IEmailService _emailService;
        private SubscriptionClient _client;

        public NotificationManager(IEmailService emailService, IConfigurationProvider config)
        {
            _emailService = emailService;
            _config = config;
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

                    // todo: filter duplicates

                    // Logging
                    var body = string.Format("Pattern match found.\n\n" +
                                             "Machine: {0}\n" +
                                             "Path: {1}\n" +
                                             "Pattern: {2}\n" +
                                             "Line number: {3}\n" +
                                             "Match: {4}\n" +
                                             "Email: {5}", machineName, logPath, pattern, lineNumber, matchLine, email);
                    Trace.TraceInformation(body);

                    // Sending email notification
                    if (email != null)
                    {
                        var notificationAddress = _config.Get(SettingNames.NotificationAddress);
                        var subject = string.Format("AwesomeLogger match found on machine {0}", machineName);
                        var htmlBody = string.Format("Machine: {0}<br /><br />" +
                                             "Path: {1}<br /><br />" +
                                             "Pattern: {2}<br /><br />" +
                                             "Line number: {3}<br /><br />" +
                                             "Match: {4}", machineName, logPath, pattern, lineNumber, matchLine);
                        var msg = new MailMessage
                        {
                            Subject = subject,
                            IsBodyHtml = true,
                            Body = htmlBody,
                            From = new MailAddress(notificationAddress),
                            To = { email.ToString() }
                        };

                        _emailService.SendAsync(msg).Wait();
                    }

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
    }
}