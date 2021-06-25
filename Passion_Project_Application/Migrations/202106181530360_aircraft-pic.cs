namespace Passion_Project_Application.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class aircraftpic : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Aircraft", "AircraftHasPic", c => c.Boolean(nullable: false));
            AddColumn("dbo.Aircraft", "PicExtension", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Aircraft", "PicExtension");
            DropColumn("dbo.Aircraft", "AircraftHasPic");
        }
    }
}
