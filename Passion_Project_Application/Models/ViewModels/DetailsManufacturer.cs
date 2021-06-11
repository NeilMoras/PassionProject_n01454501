using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Passion_Project_Application.Models.ViewModels
{
    public class DetailsManufacturer
    {    //the Manufacturer itself that we want to display
        public ManufacturerDto SelectedManufacturer { get; set; }

        //all of the related aircrafs to that particular Manufacturer
        public IEnumerable<AircraftDto> RelatedAircrafts { get; set; }
    }
}