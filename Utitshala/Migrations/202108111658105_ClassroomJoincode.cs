namespace Utitshala.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ClassroomJoincode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Classrooms", "JoinCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Classrooms", "JoinCode");
        }
    }
}
