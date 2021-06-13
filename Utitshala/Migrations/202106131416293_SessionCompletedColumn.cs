namespace Utitshala.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SessionCompletedColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sessions", "Completed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sessions", "Completed");
        }
    }
}
