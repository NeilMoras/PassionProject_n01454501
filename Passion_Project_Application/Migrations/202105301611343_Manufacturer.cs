namespace Passion_Project_Application.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Manufacturer : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Manufacturers",
                c => new
                    {
                        ManufacturerID = c.Int(nullable: false, identity: true),
                        CompanyName = c.String(),
                        Country = c.String(),
                        HeadQuaters = c.String(),
                        CompanyDescription = c.String(),
                    })
                .PrimaryKey(t => t.ManufacturerID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Manufacturers");
        }
    }
}
