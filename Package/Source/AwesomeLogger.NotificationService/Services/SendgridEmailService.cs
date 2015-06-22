using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using AwesomeLogger.NotificationService.Configuration;
using SendGrid;

namespace AwesomeLogger.NotificationService.Services
{
    internal class SendgridEmailService : IEmailService
    {
        private readonly IConfigurationProvider _config;

        public SendgridEmailService(IConfigurationProvider config)
        {
            _config = config;
        }

        public async Task SendAsync(MailMessage message)
        {
            var username = _config.Get(SettingNames.SendgridUsername);
            var password = _config.Get(SettingNames.SendgridPassword);
            if (string.IsNullOrEmpty(username) ||
                string.IsNullOrEmpty(password))
            {
                // SendGrid credentials not specified
                return;
            }

            // Create the email object first, then add the properties.
            var sendGridMessage = new SendGridMessage();

            // Add the message properties.
            sendGridMessage.From = message.From;

            // Add multiple addresses to the To field.
            IEnumerable<string> recipients = message.To.Select(t => t.ToString());
            sendGridMessage.AddTo(recipients);

            sendGridMessage.Subject = message.Subject;

            //Add the HTML and Text bodies
            sendGridMessage.Html = message.IsBodyHtml ? message.Body : null;
            sendGridMessage.Text = message.IsBodyHtml ? null : message.Body;

            // Create network credentials to access your SendGrid account.

            var credentials = new NetworkCredential(username, password);

            // Create an Web transport for sending email.
            var transportWeb = new Web(credentials);

            // Send the email asynchronously
            await transportWeb.DeliverAsync(sendGridMessage);
        }
    }
}