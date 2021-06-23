namespace Utitshala.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AssessmentChangedToObject : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Assessments", "DateCreated", c => c.DateTime(nullable: false));
            AddColumn("dbo.Assessments", "StorageURL", c => c.String());
            AddColumn("dbo.Assessments", "QuestionsCount", c => c.Int(nullable: false));
            AddColumn("dbo.Assessments", "Public", c => c.Boolean(nullable: false));
            AddColumn("dbo.Sessions", "Score", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.Assessments", "Score");
            DropColumn("dbo.Assessments", "DateTimeAttempted");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Assessments", "DateTimeAttempted", c => c.DateTime(nullable: false));
            AddColumn("dbo.Assessments", "Score", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.Sessions", "Score");
            DropColumn("dbo.Assessments", "Public");
            DropColumn("dbo.Assessments", "QuestionsCount");
            DropColumn("dbo.Assessments", "StorageURL");
            DropColumn("dbo.Assessments", "DateCreated");
        }
    }
}
