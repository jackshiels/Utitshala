namespace Utitshala.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NullableStudentRecordID : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Students", "StudentRecordID", "dbo.StudentRecords");
            DropIndex("dbo.Students", new[] { "StudentRecordID" });
            AlterColumn("dbo.Students", "StudentRecordID", c => c.Int());
            CreateIndex("dbo.Students", "StudentRecordID");
            AddForeignKey("dbo.Students", "StudentRecordID", "dbo.StudentRecords", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Students", "StudentRecordID", "dbo.StudentRecords");
            DropIndex("dbo.Students", new[] { "StudentRecordID" });
            AlterColumn("dbo.Students", "StudentRecordID", c => c.Int(nullable: false));
            CreateIndex("dbo.Students", "StudentRecordID");
            AddForeignKey("dbo.Students", "StudentRecordID", "dbo.StudentRecords", "ID", cascadeDelete: true);
        }
    }
}
