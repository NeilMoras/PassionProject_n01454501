using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Passion_Project_Application.Models
{
    public class Country
    {
        [Key]
        public int CountryID { get; set; }

        public string CountryName { get; set; }

        public string AirforceName { get; set; }

        public string CountryDescription { get; set; }

        //A country can own many aircrafts
        public ICollection<Aircraft> Aircrafts { get; set; }

    }

    public class CountryDto
    {
        public int CountryID { get; set; }

        public string CountryName { get; set; }

        public string AirforceName { get; set; }

        public string CountryDescription { get; set; }
    }
}