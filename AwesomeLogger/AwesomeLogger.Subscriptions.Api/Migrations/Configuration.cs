using System.Data.Entity.Migrations;
using AwesomeLogger.Subscriptions.Api.DAL;

namespace AwesomeLogger.Subscriptions.Api.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<SubscriptionDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "AwesomeLogger.Subscriptions.Api.DAL.SubscriptionDbContext";
        }

        protected override void Seed(SubscriptionDbContext context)
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