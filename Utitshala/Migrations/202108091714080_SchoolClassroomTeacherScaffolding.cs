namespace Utitshala.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SchoolClassroomTeacherScaffolding : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Schools",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.Classrooms", "SchoolID", c => c.Int());
            AddColumn("dbo.AspNetUsers", "SchoolID", c => c.Int(nullable: false));
            CreateIndex("dbo.Classrooms", "SchoolID");
            CreateIndex("dbo.AspNetUsers", "SchoolID");
            AddForeignKey("dbo.Classrooms", "SchoolID", "dbo.Schools", "ID");
            AddForeignKey("dbo.AspNetUsers", "SchoolID", "dbo.Schools", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "SchoolID", "dbo.Schools");
            DropForeignKey("dbo.Classrooms", "SchoolID", "dbo.Schools");
            DropIndex("dbo.AspNetUsers", new[] { "SchoolID" });
            DropIndex("dbo.Classrooms", new[] { "SchoolID" });
            DropColumn("dbo.AspNetUsers", "SchoolID");
            DropColumn("dbo.Classrooms", "SchoolID");
            DropTable("dbo.Schools");
        }
    }
}
