namespace Utitshala.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SessionLearningDesignID : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sessions", "LearningDesignID", c => c.Int(nullable: false));
            CreateIndex("dbo.Sessions", "LearningDesignID");
            AddForeignKey("dbo.Sessions", "LearningDesignID", "dbo.LearningDesigns", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Sessions", "LearningDesignID", "dbo.LearningDesigns");
            DropIndex("dbo.Sessions", new[] { "LearningDesignID" });
            DropColumn("dbo.Sessions", "LearningDesignID");
        }
    }
}
