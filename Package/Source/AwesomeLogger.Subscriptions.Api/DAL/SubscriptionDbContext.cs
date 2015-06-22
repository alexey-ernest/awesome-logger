using System.Data.Entity;

namespace AwesomeLogger.Subscriptions.Api.DAL
{
    public class SubscriptionDbContext : DbContext
    {
        public SubscriptionDbContext()
            : base("SubscriptionConnection")
        {
        }

        public DbSet<Subscription> Subscriptions { get; set; }
    }
}