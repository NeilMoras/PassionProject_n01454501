using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Passion_Project_Application.Models
{
    public class Manufacturer
    { 
        [Key]
        public int ManufacturerID { get; set; }

        public string CompanyName { get; set; }

        public string Country { get; set; }

        public string HeadQuarters { get; set; }

        public string CompanyDescription { get; set; }


    }
    public class ManufacturerDto
    {
        public int ManufacturerID { get; set; }

        public string CompanyName { get; set; }

        public string Country { get; set; }

        public string HeadQuarters { get; set; }

        public string CompanyDescription { get; set; }

    }
}