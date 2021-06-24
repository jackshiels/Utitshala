namespace Utitshala.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AssignmentName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Assignments", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Assignments", "Name");
        }
    }
}
