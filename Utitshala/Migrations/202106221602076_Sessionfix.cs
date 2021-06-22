namespace Utitshala.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Sessionfix : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Assessments", "SessionID", "dbo.Sessions");
            DropForeignKey("dbo.Sessions", "LearningDesignID", "dbo.LearningDesigns");
            DropIndex("dbo.Assessments", new[] { "SessionID" });
            DropIndex("dbo.Sessions", new[] { "LearningDesignID" });
            AddColumn("dbo.Assessments", "ClassroomID", c => c.Int());
            AddColumn("dbo.Sessions", "AssessmentID", c => c.Int());
            AddColumn("dbo.LearningDesigns", "AssessmentID", c => c.Int());
            AlterColumn("dbo.Sessions", "LearningDesignID", c => c.Int());
            CreateIndex("dbo.Assessments", "ClassroomID");
            CreateIndex("dbo.LearningDesigns", "AssessmentID");
            CreateIndex("dbo.Sessions", "LearningDesignID");
            CreateIndex("dbo.Sessions", "AssessmentID");
            AddForeignKey("dbo.LearningDesigns", "AssessmentID", "dbo.Assessments", "ID");
            AddForeignKey("dbo.Sessions", "AssessmentID", "dbo.Assessments", "ID");
            AddForeignKey("dbo.Assessments", "ClassroomID", "dbo.Classrooms", "ID");
            AddForeignKey("dbo.Sessions", "LearningDesignID", "dbo.LearningDesigns", "ID");
            DropColumn("dbo.Assessments", "SessionID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Assessments", "SessionID", c => c.Int(nullable: false));
            DropForeignKey("dbo.Sessions", "LearningDesignID", "dbo.LearningDesigns");
            DropForeignKey("dbo.Assessments", "ClassroomID", "dbo.Classrooms");
            DropForeignKey("dbo.Sessions", "AssessmentID", "dbo.Assessments");
            DropForeignKey("dbo.LearningDesigns", "AssessmentID", "dbo.Assessments");
            DropIndex("dbo.Sessions", new[] { "AssessmentID" });
            DropIndex("dbo.Sessions", new[] { "LearningDesignID" });
            DropIndex("dbo.LearningDesigns", new[] { "AssessmentID" });
            DropIndex("dbo.Assessments", new[] { "ClassroomID" });
            AlterColumn("dbo.Sessions", "LearningDesignID", c => c.Int(nullable: false));
            DropColumn("dbo.LearningDesigns", "AssessmentID");
            DropColumn("dbo.Sessions", "AssessmentID");
            DropColumn("dbo.Assessments", "ClassroomID");
            CreateIndex("dbo.Sessions", "LearningDesignID");
            CreateIndex("dbo.Assessments", "SessionID");
            AddForeignKey("dbo.Sessions", "LearningDesignID", "dbo.LearningDesigns", "ID", cascadeDelete: true);
            AddForeignKey("dbo.Assessments", "SessionID", "dbo.Sessions", "ID", cascadeDelete: true);
        }
    }
}
