namespace Passion_Project_Application.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class aircraftmanufacturer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Aircraft", "ManufacturerID", c => c.Int(nullable: false));
            CreateIndex("dbo.Aircraft", "ManufacturerID");
            AddForeignKey("dbo.Aircraft", "ManufacturerID", "dbo.Manufacturers", "ManufacturerID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Aircraft", "ManufacturerID", "dbo.Manufacturers");
            DropIndex("dbo.Aircraft", new[] { "ManufacturerID" });
            DropColumn("dbo.Aircraft", "ManufacturerID");
        }
    }
}
