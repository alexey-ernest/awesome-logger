using System.Threading.Tasks;
using AwesomeLogger.NotificationService.Models;

namespace AwesomeLogger.NotificationService.Services
{
    public interface IAuditService
    {
        Task AddAsync(PatternMatchModel match);
    }
}