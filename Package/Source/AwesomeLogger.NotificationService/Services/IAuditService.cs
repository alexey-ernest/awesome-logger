using System.Threading.Tasks;
using AwesomeLogger.NotificationService.Models;

namespace AwesomeLogger.NotificationService.Services
{
    internal interface IAuditService
    {
        Task AddAsync(PatternMatchModel match);
    }
}