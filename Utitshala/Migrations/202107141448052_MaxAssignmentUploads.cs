namespace Utitshala.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MaxAssignmentUploads : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Assignments", "MaxUploads", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Assignments", "MaxUploads");
        }
    }
}
