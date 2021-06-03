namespace Utitshala.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StudentServiceIDString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Students", "ServiceUserID", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Students", "ServiceUserID", c => c.Int(nullable: false));
        }
    }
}
