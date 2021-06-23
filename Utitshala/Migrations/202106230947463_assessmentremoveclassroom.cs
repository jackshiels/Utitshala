namespace Utitshala.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class assessmentremoveclassroom : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Assessments", name: "ClassroomID", newName: "Classroom_ID");
            RenameIndex(table: "dbo.Assessments", name: "IX_ClassroomID", newName: "IX_Classroom_ID");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Assessments", name: "IX_Classroom_ID", newName: "IX_ClassroomID");
            RenameColumn(table: "dbo.Assessments", name: "Classroom_ID", newName: "ClassroomID");
        }
    }
}
