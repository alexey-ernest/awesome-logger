using System.Data.Entity;

namespace AwesomeLogger.Audit.Api.DAL
{
    public class AuditDbContext : DbContext
    {
        public AuditDbContext()
            : base("AuditConnection")
        {
        }

        public DbSet<PatternMatch> Matches { get; set; }
    }
}