namespace Utitshala.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ClassroomLimitTemp : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AspNetUsers", "SchoolID", "dbo.Schools");
            DropIndex("dbo.AspNetUsers", new[] { "SchoolID" });
            AddColumn("dbo.AspNetUsers", "ClassroomID", c => c.Int(nullable: false));
            CreateIndex("dbo.AspNetUsers", "ClassroomID");
            AddForeignKey("dbo.AspNetUsers", "ClassroomID", "dbo.Classrooms", "ID", cascadeDelete: true);
            DropColumn("dbo.AspNetUsers", "SchoolID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "SchoolID", c => c.Int(nullable: false));
            DropForeignKey("dbo.AspNetUsers", "ClassroomID", "dbo.Classrooms");
            DropIndex("dbo.AspNetUsers", new[] { "ClassroomID" });
            DropColumn("dbo.AspNetUsers", "ClassroomID");
            CreateIndex("dbo.AspNetUsers", "SchoolID");
            AddForeignKey("dbo.AspNetUsers", "SchoolID", "dbo.Schools", "ID", cascadeDelete: true);
        }
    }
}
