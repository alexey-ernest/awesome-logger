using System.Data.Entity.Migrations;
using AwesomeLogger.Audit.Api.DAL;

namespace AwesomeLogger.Audit.Api.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<AuditDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "AwesomeLogger.Audit.Api.DAL.AuditDbContext";
        }

        protected override void Seed(AuditDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}