namespace Passion_Project_Application.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ts : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Manufacturers", "HeadQuarters", c => c.String());
            DropColumn("dbo.Manufacturers", "HeadQuaters");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Manufacturers", "HeadQuaters", c => c.String());
            DropColumn("dbo.Manufacturers", "HeadQuarters");
        }
    }
}
