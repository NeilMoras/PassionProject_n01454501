namespace Passion_Project_Application.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class aircraftengine : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Aircraft", "Engine", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Aircraft", "Engine");
        }
    }
}
