using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Passion_Project_Application.Models.ViewModels
{
    public class UpdateAircraft
    {
        //This viewmodel is a class which stores information that we need to present to /Aircraft/Update/{}

        //the existing aircraft information

        public AircraftDto SelectedAircraft { get; set; }

        // all manufacturers to choose from when updating this aircraft

        public IEnumerable<ManufacturerDto> ManufacturerOptions { get; set; }
    }
}
