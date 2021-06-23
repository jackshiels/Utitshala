namespace Utitshala.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AssignmentInitial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Assignments",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        DateOpened = c.DateTime(nullable: false),
                        DateDue = c.DateTime(),
                        Type = c.Int(nullable: false),
                        Public = c.Boolean(nullable: false),
                        ClassroomID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Classrooms", t => t.ClassroomID)
                .Index(t => t.ClassroomID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Assignments", "ClassroomID", "dbo.Classrooms");
            DropIndex("dbo.Assignments", new[] { "ClassroomID" });
            DropTable("dbo.Assignments");
        }
    }
}
