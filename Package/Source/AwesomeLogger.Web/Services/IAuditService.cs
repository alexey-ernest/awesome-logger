using System.Collections.Generic;
using System.Threading.Tasks;
using AwesomeLogger.Web.Models;

namespace AwesomeLogger.Web.Services
{
    public interface IAuditService
    {
        Task<IEnumerable<PatternMatchModel>> SearchAsync(string machineName, string searchPath, string pattern, string email, int skip, int top);
    }
}