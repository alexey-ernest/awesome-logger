using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AwesomeLogger.Audit.Api.Exceptions;

namespace AwesomeLogger.Audit.Api.DAL
{
    public class PatternMatchRepository : IPatternMatchRepository
    {
        private readonly AuditDbContext _db;

        public PatternMatchRepository(AuditDbContext db)
        {
            _db = db;
        }

        public void Dispose()
        {
            _db.Dispose();
        }

        public async Task<PatternMatch> AddAsync(PatternMatch match)
        {
            // checking if already exists
            var matchPattern = match;
            var identical =
                await
                    _db.Matches.Where(
                        m => string.Equals(m.MachineName, matchPattern.MachineName, StringComparison.OrdinalIgnoreCase) &&
                             string.Equals(m.SearchPath, matchPattern.SearchPath) &&
                             string.Equals(m.LogPath, matchPattern.LogPath) &&
                             string.Equals(m.Pattern, matchPattern.Pattern) &&
                             m.Line == matchPattern.Line &&
                             string.Equals(m.Email, matchPattern.Email, StringComparison.OrdinalIgnoreCase) &&
                             string.Equals(m.Match, matchPattern.Match)
                        ).FirstOrDefaultAsync();
            if (identical != null)
            {
                throw new ConflictException("Pattern match with the same parameters already exists.");
            }

            match.Created = DateTime.UtcNow;

            match = _db.Matches.Add(match);
            await _db.SaveChangesAsync();
            return match;
        }

        public async Task<IEnumerable<PatternMatch>> GetRelatedAsync(string machine, string searchPath, string pattern,
            string email)
        {
            return await
                _db.Matches.Where(m => string.Equals(m.MachineName, machine, StringComparison.OrdinalIgnoreCase) &&
                                       string.Equals(m.SearchPath, searchPath) &&
                                       string.Equals(m.Pattern, pattern) &&
                                       string.Equals(m.Email, email, StringComparison.OrdinalIgnoreCase))
                    .ToListAsync();
        }
    }
}