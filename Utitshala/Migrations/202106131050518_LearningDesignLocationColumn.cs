namespace Utitshala.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LearningDesignLocationColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LearningDesigns", "StorageURL", c => c.String());
            DropColumn("dbo.LearningDesigns", "StorageID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.LearningDesigns", "StorageID", c => c.Int(nullable: false));
            DropColumn("dbo.LearningDesigns", "StorageURL");
        }
    }
}
