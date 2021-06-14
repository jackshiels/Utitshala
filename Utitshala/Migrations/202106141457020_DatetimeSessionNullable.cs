namespace Utitshala.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DatetimeSessionNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Sessions", "DateTimeStarted", c => c.DateTime());
            AlterColumn("dbo.Sessions", "DateTimeEnded", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Sessions", "DateTimeEnded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Sessions", "DateTimeStarted", c => c.DateTime(nullable: false));
        }
    }
}
