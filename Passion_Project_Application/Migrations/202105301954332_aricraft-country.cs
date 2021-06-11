namespace Passion_Project_Application.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class aricraftcountry : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        CountryID = c.Int(nullable: false, identity: true),
                        CountryName = c.String(),
                        CountryDescription = c.String(),
                    })
                .PrimaryKey(t => t.CountryID);
            
            CreateTable(
                "dbo.CountryAircrafts",
                c => new
                    {
                        Country_CountryID = c.Int(nullable: false),
                        Aircraft_AircraftID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Country_CountryID, t.Aircraft_AircraftID })
                .ForeignKey("dbo.Countries", t => t.Country_CountryID, cascadeDelete: true)
                .ForeignKey("dbo.Aircraft", t => t.Aircraft_AircraftID, cascadeDelete: true)
                .Index(t => t.Country_CountryID)
                .Index(t => t.Aircraft_AircraftID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CountryAircrafts", "Aircraft_AircraftID", "dbo.Aircraft");
            DropForeignKey("dbo.CountryAircrafts", "Country_CountryID", "dbo.Countries");
            DropIndex("dbo.CountryAircrafts", new[] { "Aircraft_AircraftID" });
            DropIndex("dbo.CountryAircrafts", new[] { "Country_CountryID" });
            DropTable("dbo.CountryAircrafts");
            DropTable("dbo.Countries");
        }
    }
}
