using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Passion_Project_Application.Models.ViewModels
{
    public class DetailsAircraft
    {
        public AircraftDto SelectedAircraft { get; set; }
        public IEnumerable<CountryDto> ResponsibleCountries { get; set; }

        public IEnumerable<CountryDto> AvailableCountries { get; set; }
    }
}