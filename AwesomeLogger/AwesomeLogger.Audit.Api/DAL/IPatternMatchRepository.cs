using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AwesomeLogger.Audit.Api.DAL
{
    public interface IPatternMatchRepository : IDisposable
    {
        Task<PatternMatch> AddAsync(PatternMatch match);

        Task<IEnumerable<PatternMatch>> GetRelatedAsync(string machine, string searchPath, string pattern, string email);
    }
}