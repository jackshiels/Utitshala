namespace Utitshala.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ServiceIDStudent : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Students", "ServiceUserID", c => c.Int(nullable: false));
            DropColumn("dbo.Students", "TelegramUserID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Students", "TelegramUserID", c => c.Int(nullable: false));
            DropColumn("dbo.Students", "ServiceUserID");
        }
    }
}
