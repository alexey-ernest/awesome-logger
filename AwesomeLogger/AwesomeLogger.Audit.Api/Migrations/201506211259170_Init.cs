namespace AwesomeLogger.Audit.Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PatternMatches",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Created = c.DateTime(nullable: false),
                        MachineName = c.String(nullable: false, maxLength: 200),
                        SearchPath = c.String(nullable: false, maxLength: 255),
                        LogPath = c.String(nullable: false, maxLength: 255),
                        Pattern = c.String(nullable: false, maxLength: 200),
                        Line = c.Int(nullable: false),
                        Email = c.String(nullable: false, maxLength: 255),
                        Match = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PatternMatches");
        }
    }
}
