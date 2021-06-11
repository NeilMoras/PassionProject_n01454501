using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Passion_Project_Application.Models.ViewModels
{
    public class DetailsCountry
    {
        public CountryDto SelectedCountry { get; set; }
        public IEnumerable<AircraftDto> KeptAircrafts { get; set; }
    }
}