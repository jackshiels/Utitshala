namespace Utitshala.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StudentAssignments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StudentAssignments",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UploadDate = c.DateTime(nullable: false),
                        Value = c.String(),
                        MetaData = c.String(),
                        Type = c.Int(nullable: false),
                        AssignmentID = c.Int(),
                        FileSize = c.Int(),
                        StudentRecord_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Assignments", t => t.AssignmentID)
                .ForeignKey("dbo.StudentRecords", t => t.StudentRecord_ID)
                .Index(t => t.AssignmentID)
                .Index(t => t.StudentRecord_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StudentAssignments", "StudentRecord_ID", "dbo.StudentRecords");
            DropForeignKey("dbo.StudentAssignments", "AssignmentID", "dbo.Assignments");
            DropIndex("dbo.StudentAssignments", new[] { "StudentRecord_ID" });
            DropIndex("dbo.StudentAssignments", new[] { "AssignmentID" });
            DropTable("dbo.StudentAssignments");
        }
    }
}
