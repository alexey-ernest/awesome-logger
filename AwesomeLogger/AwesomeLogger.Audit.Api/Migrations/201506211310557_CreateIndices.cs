namespace AwesomeLogger.Audit.Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateIndices : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PatternMatches", "Match", c => c.String(nullable: false, maxLength: 1024));
            CreateIndex("dbo.PatternMatches", new[] { "MachineName", "SearchPath", "Pattern", "Email", "LogPath", "Line", "Match" }, name: "IX_SearchIndex");
            CreateIndex("dbo.PatternMatches", new[] { "MachineName", "SearchPath", "Pattern", "Email", "Created" }, name: "IX_Subscription");
        }
        
        public override void Down()
        {
            DropIndex("dbo.PatternMatches", "IX_Subscription");
            DropIndex("dbo.PatternMatches", "IX_SearchIndex");
            AlterColumn("dbo.PatternMatches", "Match", c => c.String(nullable: false));
        }
    }
}
