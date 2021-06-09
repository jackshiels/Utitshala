namespace Utitshala.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DateStudentJoined : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Students", "DateJoined", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Students", "DateJoined");
        }
    }
}
