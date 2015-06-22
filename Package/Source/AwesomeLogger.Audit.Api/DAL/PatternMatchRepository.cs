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
                        m => m.MachineName == matchPattern.MachineName &&
                             m.SearchPath == matchPattern.SearchPath &&
                             m.Pattern == matchPattern.Pattern &&
                             m.Email == matchPattern.Email &&
                             m.LogPath == matchPattern.LogPath &&
                             m.Line == matchPattern.Line &&
                             m.Match == matchPattern.Match
                        ).FirstOrDefaultAsync();
            if (identical != null)
            {
                throw new ConflictException("Pattern match with the same parameters already exists.");
            }

            match = _db.Matches.Add(match);
            await _db.SaveChangesAsync();
            return match;
        }

        public IQueryable<PatternMatch> Query(string machine, string searchPath, string pattern,
            string email)
        {
            return
                _db.Matches.Where(m => m.MachineName == machine &&
                                       m.SearchPath == searchPath &&
                                       m.Pattern == pattern &&
                                       m.Email == email)
                    .OrderByDescending(m => m.Created);
        }
    }
}