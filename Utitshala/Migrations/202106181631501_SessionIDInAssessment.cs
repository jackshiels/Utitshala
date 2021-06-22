namespace Utitshala.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SessionIDInAssessment : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Assessments", "Session_ID", "dbo.Sessions");
            DropIndex("dbo.Assessments", new[] { "Session_ID" });
            RenameColumn(table: "dbo.Assessments", name: "Session_ID", newName: "SessionID");
            AlterColumn("dbo.Assessments", "SessionID", c => c.Int(nullable: false));
            CreateIndex("dbo.Assessments", "SessionID");
            AddForeignKey("dbo.Assessments", "SessionID", "dbo.Sessions", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Assessments", "SessionID", "dbo.Sessions");
            DropIndex("dbo.Assessments", new[] { "SessionID" });
            AlterColumn("dbo.Assessments", "SessionID", c => c.Int());
            RenameColumn(table: "dbo.Assessments", name: "SessionID", newName: "Session_ID");
            CreateIndex("dbo.Assessments", "Session_ID");
            AddForeignKey("dbo.Assessments", "Session_ID", "dbo.Sessions", "ID");
        }
    }
}
