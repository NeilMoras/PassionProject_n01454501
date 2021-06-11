namespace Passion_Project_Application.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updedatbase : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Countries", "AirforceName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Countries", "AirforceName");
        }
    }
}
