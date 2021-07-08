namespace Utitshala.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveAssignmentTypefromStudentAssignment : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.StudentAssignments", "Type");
        }
        
        public override void Down()
        {
            AddColumn("dbo.StudentAssignments", "Type", c => c.Int(nullable: false));
        }
    }
}
