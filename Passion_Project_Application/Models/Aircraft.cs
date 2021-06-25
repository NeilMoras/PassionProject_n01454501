using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Passion_Project_Application.Models
{
    public class Aircraft
    {
        [Key]
        public int AircraftID { get; set; }
        public string AircraftName { get; set; }

        public string AircraftType { get; set; }
        public DateTime YearIntroduced { get; set; }

        //Speed in Miles Per Hour
        public int MaxSpeed { get; set; }
        //Range in Kilometers
        public int Range { get; set; }

        public string Engine { get; set; }

        public string Description { get; set; }

        //data for keeping track of the animal images uploaded
        //images uplaoded and stored into /Content/Images/Aircrafts/{id}.{extention}
        public bool AircraftHasPic { get; set; }
        public string PicExtension { get; set; }

        //An Aircraft can have one manufacturer 
        //A manufacturer can have many Aircrafts
        [ForeignKey("Manufacturers")]
        public int ManufacturerID { get; set; }
        public virtual Manufacturer Manufacturers { get; set; }

        //An aircraft can be owned by many countries
        public ICollection<Country> Countries { get; set; }



    }

    public class AircraftDto
    {
        public int AircraftID { get; set; }
        public string AircraftName { get; set; }

        public string AircraftType { get; set; }
        public DateTime YearIntroduced { get; set; }

        //Speed in Miles Per Hour
        public int MaxSpeed { get; set; }
        //Range in Kilometers
        public int Range { get; set; }

        public string Engine { get; set; }

        public string Description { get; set; }

        public int ManufacturerID { get; set; }
        public string CompanyName { get; set; }


        //data for keeping track of the animal images uploaded
        //images uplaoded and stored into /Content/Images/Aircrafts/{id}.{extention}
        public bool AircraftHasPic { get; set; }
        public string PicExtension { get; set; }
    }
}