namespace Passion_Project_Application.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class manufactrerPic : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Manufacturers", "ManufacturerHasPic", c => c.Boolean(nullable: false));
            AddColumn("dbo.Manufacturers", "ManufacturerPicExtension", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Manufacturers", "ManufacturerPicExtension");
            DropColumn("dbo.Manufacturers", "ManufacturerHasPic");
        }
    }
}
