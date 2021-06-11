using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Diagnostics;
using Passion_Project_Application.Models;
using Passion_Project_Application.Models.ViewModels;
using System.Web.Script.Serialization;

namespace Passion_Project_Application.Controllers
{
    public class ManufacturerController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static ManufacturerController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44384/api/");
        }
        
        // GET: Manufacturer/List
        public ActionResult List()

        {
            //objective: communicate with our Manufacturer data api to retrieve a list fo Manufacturers
            //curl https://localhost:44384/api/manufacturerdata/ListManufacturers

            string url = "manufacturerdata/listmanufacturers";
            HttpResponseMessage response = client.GetAsync(url).Result;


            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode);

            IEnumerable<ManufacturerDto> Manufacturers = response.Content.ReadAsAsync<IEnumerable<ManufacturerDto>>().Result;
            //Debug.WriteLine("Number of Manufacturer received : ");
            //Debug.WriteLine(Manufactrer.Count());

            return View(Manufacturers);
        }

        // GET: Manufacturer/Details/5
        public ActionResult Details(int id)
        {
            //objective: communicate with our Manufacturer data api to retrieve one Manufacturer
            //curl https://localhost:44384/api/manufacturerdata/findmanufacturer/{id}

            DetailsManufacturer ViewModel = new DetailsManufacturer();

            string url = "manufacturerdata/findmanufacturer/"+id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            Debug.WriteLine("The response code is ");
            Debug.WriteLine(response.StatusCode);

            ManufacturerDto SelectedManufacturer = response.Content.ReadAsAsync<ManufacturerDto>().Result;
            Debug.WriteLine("Manufacturer received : ");
            Debug.WriteLine(SelectedManufacturer.CompanyName);

            ViewModel.SelectedManufacturer = SelectedManufacturer;

            //showcase information about aircrafts related to thisManufacturer
            //send a request to gather information about aircrafts related to a particular maunfacturer ID
            url = "aircraftdata/listaircraftsformanufacturer/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<AircraftDto> RelatedAircrafts = response.Content.ReadAsAsync<IEnumerable<AircraftDto>>().Result;

            ViewModel.RelatedAircrafts = RelatedAircrafts;


            return View(ViewModel);
        }
            public ActionResult Error()
        {
            return View();
        }

        // GET: Manufacturer/New
        public ActionResult New()
        {
            return View();
        }

        // POST: Manufacturer/Create
        [HttpPost]
        public ActionResult Create(Manufacturer manufacturer)
        {
            Debug.WriteLine("the json payload is :");
            //Debug.WriteLine(Manufacturer.CompanyName);
            //objective: add a new Manufacturer into our system using the API
            //curl -H "Content-Type:application/json" -d @Manufacturer.json https://localhost:44324/api/Manufacturerdata/addmanufacturer 
            string url = "manufacturerdata/addmanufacturer";


            string jsonpayload = jss.Serialize(manufacturer);
            Debug.WriteLine(jsonpayload);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }

        }

        // GET: Manufacturer/Edit/5
        public ActionResult Edit(int id)
        {

            string url = "manufacturerdata/findmanufacturer/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            ManufacturerDto selectedManufacturer = response.Content.ReadAsAsync<ManufacturerDto>().Result;
            return View(selectedManufacturer);
        }

        // POST: Manufacturer/Update/5
        [HttpPost]
        public ActionResult Update(int id, Manufacturer Manufacturer)
        {
            string url = "manufacturerdata/updatemanufacturer/" + id;
            string jsonpayload = jss.Serialize(Manufacturer);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            Debug.WriteLine(content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Manufacturer/DeleteConfirm/5
        public ActionResult DeleteConfirm(int id)
        {
            string url = "manufacturerdata/findmanufacturer/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            ManufacturerDto selectedManufacturer = response.Content.ReadAsAsync<ManufacturerDto>().Result;
            return View(selectedManufacturer);
        }

        // POST: Manufacturer/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            string url = "manufacturerdata/deletemanufacturer/" + id;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
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
