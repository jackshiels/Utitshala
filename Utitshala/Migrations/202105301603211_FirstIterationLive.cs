namespace Utitshala.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FirstIterationLive : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Assessments",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Score = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateTimeAttempted = c.DateTime(nullable: false),
                        Session_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Sessions", t => t.Session_ID)
                .Index(t => t.Session_ID);
            
            CreateTable(
                "dbo.LearningDesigns",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        StorageID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Sessions",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        DateTimeStarted = c.DateTime(nullable: false),
                        DateTimeEnded = c.DateTime(nullable: false),
                        Abandoned = c.Boolean(nullable: false),
                        StudentRecord_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.StudentRecords", t => t.StudentRecord_ID)
                .Index(t => t.StudentRecord_ID);
            
            CreateTable(
                "dbo.StudentRecords",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Students",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TelegramUserID = c.Int(nullable: false),
                        StudentRecordID = c.Int(nullable: false),
                        Name = c.String(),
                        Language = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.StudentRecords", t => t.StudentRecordID, cascadeDelete: true)
                .Index(t => t.StudentRecordID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Students", "StudentRecordID", "dbo.StudentRecords");
            DropForeignKey("dbo.Sessions", "StudentRecord_ID", "dbo.StudentRecords");
            DropForeignKey("dbo.Assessments", "Session_ID", "dbo.Sessions");
            DropIndex("dbo.Students", new[] { "StudentRecordID" });
            DropIndex("dbo.Sessions", new[] { "StudentRecord_ID" });
            DropIndex("dbo.Assessments", new[] { "Session_ID" });
            DropTable("dbo.Students");
            DropTable("dbo.StudentRecords");
            DropTable("dbo.Sessions");
            DropTable("dbo.LearningDesigns");
            DropTable("dbo.Assessments");
        }
    }
}
