namespace Utitshala.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Classrooms : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Classrooms",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.LearningDesigns", "Public", c => c.Boolean(nullable: false));
            AddColumn("dbo.LearningDesigns", "ClassroomID", c => c.Int());
            AddColumn("dbo.Students", "ClassroomID", c => c.Int());
            CreateIndex("dbo.LearningDesigns", "ClassroomID");
            CreateIndex("dbo.Students", "ClassroomID");
            AddForeignKey("dbo.LearningDesigns", "ClassroomID", "dbo.Classrooms", "ID");
            AddForeignKey("dbo.Students", "ClassroomID", "dbo.Classrooms", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Students", "ClassroomID", "dbo.Classrooms");
            DropForeignKey("dbo.LearningDesigns", "ClassroomID", "dbo.Classrooms");
            DropIndex("dbo.Students", new[] { "ClassroomID" });
            DropIndex("dbo.LearningDesigns", new[] { "ClassroomID" });
            DropColumn("dbo.Students", "ClassroomID");
            DropColumn("dbo.LearningDesigns", "ClassroomID");
            DropColumn("dbo.LearningDesigns", "Public");
            DropTable("dbo.Classrooms");
        }
    }
}
