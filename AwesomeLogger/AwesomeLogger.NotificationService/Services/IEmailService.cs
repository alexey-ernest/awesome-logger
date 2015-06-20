using System.Net.Mail;
using System.Threading.Tasks;

namespace AwesomeLogger.NotificationService.Services
{
    internal interface IEmailService
    {
        Task SendAsync(MailMessage message);
    }
}