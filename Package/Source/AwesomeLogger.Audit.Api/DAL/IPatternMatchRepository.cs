using System;
using System.Linq;
using System.Threading.Tasks;

namespace AwesomeLogger.Audit.Api.DAL
{
    public interface IPatternMatchRepository : IDisposable
    {
        Task<PatternMatch> AddAsync(PatternMatch match);

        IQueryable<PatternMatch> Query(string machine, string searchPath, string pattern, string email);
    }
}