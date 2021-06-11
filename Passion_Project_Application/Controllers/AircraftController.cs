using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using Passion_Project_Application.Models;
using System.Web.Script.Serialization;
using Passion_Project_Application.Models.ViewModels;
using System.Diagnostics;

namespace Passion_Project_Application.Controllers
{
    public class AircraftController : Controller

    {
        private static readonly HttpClient client;
         private JavaScriptSerializer jss = new JavaScriptSerializer();

        static AircraftController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44384/api/");
        }
        // GET: Aircraft/List
        public ActionResult List()
        {
            //Objective : communicate without our aircraft data api to retrive a list of aircrafts
            //curl https://localhost:44384/api/aircraftdata/listaircrafts

    
            string url = "aircraftdata/listaircrafts";
            HttpResponseMessage response = client.GetAsync(url).Result;


            IEnumerable<AircraftDto> aircrafts = response.Content.ReadAsAsync<IEnumerable<AircraftDto>>().Result;

            return View(aircrafts);
        }

        // GET: Aircraft/Details/5
        public ActionResult Details(int id)
        {
            DetailsAircraft ViewModel = new DetailsAircraft();

            //Objective : communicate without our aircraft data api to retrive a one aircrafts
            //curl https://localhost:44384/api/aircraftdata/findaircraft/{id}

           
            string url = "aircraftdata/findaircraft/"+id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            Debug.WriteLine("The response code is ");
            Debug.WriteLine(response.StatusCode);

            AircraftDto SelectedAircraft = response.Content.ReadAsAsync<AircraftDto>().Result;
            Debug.WriteLine("Aircarft received : ");
            Debug.WriteLine(SelectedAircraft.AircraftName);

            ViewModel.SelectedAircraft = SelectedAircraft;

            ///Show associated countries with this aircraft
            url = "countrydata/listcountriesforaircraft/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<CountryDto> ResponsibleCountries = response.Content.ReadAsAsync<IEnumerable<CountryDto>>().Result;

            ViewModel.ResponsibleCountries = ResponsibleCountries;

            url = "countrydata/listcountriesnotoperatingaircraft/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<CountryDto> AvailableCountries = response.Content.ReadAsAsync<IEnumerable<CountryDto>>().Result;

            ViewModel.AvailableCountries = AvailableCountries;

            return View(ViewModel);
        }

        //POST: Aircraft/Associate/{aircraftid}
        [HttpPost]
        public ActionResult Associate(int id, int CountryID)
        {
            Debug.WriteLine("Attempting to associate aircraft :" + id + " with country " + CountryID);

            //call our api to associate aircraft with country
            string url = "aircraftdata/associateaircraftwithcountry/" + id + "/" + CountryID;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("Details/" + id);
        }

        //POST: Aircraft/UnAssociate/{id}?CountryID={CountryID}
        [HttpGet]
        public ActionResult UnAssociate(int id, int CountryID)
        {
            Debug.WriteLine("Attempting to unassociate aircraft :" + id + " with country " + CountryID);

            //call our api to associate aircraft with country
            string url = "aircraftdata/unassociateaircraftwithcountry/" + id + "/" + CountryID;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("Details/" + id);
        }

        public ActionResult Error() {
            return View();
        }
        
        // GET: Aircraft/New
        public ActionResult New()
        {
            //informtaiton about all Aircraft in the system
           //information about all manufacturer in the system.
            //GET api/manufacturerdata/listmanufacturer

            string url = "manufacturerdata/listmanufacturers";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<ManufacturerDto> ManufacturerOptions = response.Content.ReadAsAsync<IEnumerable<ManufacturerDto>>().Result;

            return View(ManufacturerOptions);
        }

        // POST: Aircraft/Create
        [HttpPost]
        public ActionResult Create(Aircraft aircraft)

        {
            Debug.WriteLine("the json payload is : ");
            //objective: AdditionalMetadataAttribute a new aircraft into our system using API
            //curl -H "Content-Type:application/json" -d @aircraft.json https://localhost:44384/api/aircraftdata/addaircraft
            string url = "aircraftdata/addaircraft";

            
          
            string jsonpayload = jss.Serialize(aircraft);
              Debug.WriteLine(jsonpayload);
            HttpContent content =  new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if( response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
           
        }

        // GET: Aircraft/Edit/5
        public ActionResult Edit(int id)
        {
            UpdateAircraft ViewModel = new UpdateAircraft();

            //the existing aircarft information
            string url = "aircraftdata/findaircraft/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            AircraftDto SelectedAircraft = response.Content.ReadAsAsync<AircraftDto>().Result;
            ViewModel.SelectedAircraft = SelectedAircraft;

            // all Manufacturers to choose from when updating this aircraft
            //the existing aircraft information
            url = "manufacturerdata/listmanufacturers/";
            response = client.GetAsync(url).Result;
            IEnumerable<ManufacturerDto> ManufacturerOptions = response.Content.ReadAsAsync<IEnumerable<ManufacturerDto>>().Result;

            ViewModel.ManufacturerOptions = ManufacturerOptions;

            return View(ViewModel);
        }

        // POST: Aircraft/Update/5
        [HttpPost]
        public ActionResult Update(int id, Aircraft aircraft)
        {
            string url = "aircraftdata/updateaircraft/" + id;
         
            string jsonpayload = jss.Serialize(aircraft);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            Debug.WriteLine(content);
            if(response.IsSuccessStatusCode)
           
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Aircraft/Delete/5
        public ActionResult DeleteConfirm(int id)
        {
            string url = "aircraftdata/findaircraft/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            AircraftDto selectedaircraft = response.Content.ReadAsAsync<AircraftDto>().Result;
            return View(selectedaircraft);
        }

        // POST: Aircraft/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            string url = "aircraftdata/deleteaircraft/" + id;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
          if(response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
    }
}
