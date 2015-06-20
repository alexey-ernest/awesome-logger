using System.Data.Entity.Migrations;

namespace AwesomeLogger.Subscriptions.Api.Migrations
{
    public partial class AddIndices : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Subscriptions", "Created");

            AlterColumn("dbo.Subscriptions", "MachineName", c => c.String(false, 200));
            CreateIndex("dbo.Subscriptions", "MachineName");
        }

        public override void Down()
        {
            DropIndex("dbo.Subscriptions", new[] {"MachineName"});
            AlterColumn("dbo.Subscriptions", "MachineName", c => c.String(false));

            DropIndex("dbo.Subscriptions", new[] {"Created"});
        }
    }
}