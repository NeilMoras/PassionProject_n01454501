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
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                //cookies are manually set in RequestHeader
                UseCookies = false
            };

            client = new HttpClient(handler);
            client.BaseAddress = new Uri("https://localhost:44384/api/");
        }

        /// <summary>
        /// Grabs the authentication cookie sent to this controller.
        /// For proper WebAPI authentication, you can send a post request with login credentials to the WebAPI and log the access token from the response. The controller already knows this token, so we're just passing it up the chain.
        /// </summary>
        private void GetApplicationCookie()
        {
            string token = "";
            //HTTP client is set up to be reused, otherwise it will exhaust server resources.
            //This is a bit dangerous because a previously authenticated cookie could be cached for
            //a follow-up request from someone else. Reset cookies in HTTP client before grabbing a new one.
            client.DefaultRequestHeaders.Remove("Cookie");
            if (!User.Identity.IsAuthenticated) return;

            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get(".AspNet.ApplicationCookie");
            if (cookie != null) token = cookie.Value;

            //collect token as it is submitted to the controller
            //use it to pass along to the WebAPI.
            Debug.WriteLine("Token Submitted is : " + token);
            if (token != "") client.DefaultRequestHeaders.Add("Cookie", ".AspNet.ApplicationCookie=" + token);

            return;
        }
           
        
        // GET: Aircraft/List
        public ActionResult List(string search)
        {
            //Objective : communicate without our aircraft data api to retrive a list of aircrafts
            //curl https://localhost:44384/api/aircraftdata/listaircrafts

    
            string url = "aircraftdata/listaircrafts";
            HttpResponseMessage response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {


                IEnumerable<AircraftDto> SelectedAircraft = response.Content.ReadAsAsync<IEnumerable<AircraftDto>>().Result;
                return View(search == null ? SelectedAircraft :
                    SelectedAircraft.Where(x => x.AircraftName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0).ToList());
             }
        
            else
            {
                return RedirectToAction("Error");
    }
}


// GET: Aircraft/Details/5
public ActionResult Details(int id)
        {
            DetailsAircraft ViewModel = new DetailsAircraft();

            //Objective : communicate without our aircraft data api to retrive  one aircraft
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
    [Authorize]
    public ActionResult Associate(int id, int CountryID)
        {
        GetApplicationCookie();//get token credentials
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
    [Authorize]
    public ActionResult UnAssociate(int id, int CountryID)
        {
        GetApplicationCookie();//get token credentials
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

    [Authorize]
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
    [Authorize]
    public ActionResult Create(Aircraft aircraft)

        {
        GetApplicationCookie();//get token credentials
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
    [Authorize]
    public ActionResult Edit(int id)
        {
            UpdateAircraft ViewModel = new UpdateAircraft();

            //the existing aircarft information
            string url = "aircraftdata/findaircraft/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            AircraftDto SelectedAircraft = response.Content.ReadAsAsync<AircraftDto>().Result;
            ViewModel.SelectedAircraft = SelectedAircraft;

            // all Manufacturers to choose from  a dropdown list when updating this aircraft
            //the existing aircraft information
            url = "manufacturerdata/listmanufacturers/";
            response = client.GetAsync(url).Result;
            IEnumerable<ManufacturerDto> ManufacturerOptions = response.Content.ReadAsAsync<IEnumerable<ManufacturerDto>>().Result;

            ViewModel.ManufacturerOptions = ManufacturerOptions;

            return View(ViewModel);
        }

        // POST: Aircraft/Update/5
        [HttpPost]
    [Authorize]
    public ActionResult Update(int id, Aircraft aircraft, HttpPostedFileBase AircraftPic)
        {
        GetApplicationCookie();//get token credentials
        string url = "aircraftdata/updateaircraft/" + id;
         
            string jsonpayload = jss.Serialize(aircraft);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            Debug.WriteLine(content);
            if(response.IsSuccessStatusCode && AircraftPic != null)
           
            {
                //Send over image data for Aircraft
                url = "AircraftData/UploadAircraftPic/" + id;
                //Debug.WriteLine("Received Aircraft Picture "+AircraftPic.FileName);

                MultipartFormDataContent requestcontent = new MultipartFormDataContent();
                HttpContent imagecontent = new StreamContent(AircraftPic.InputStream);
                requestcontent.Add(imagecontent, "AircraftPic", AircraftPic.FileName);
                response = client.PostAsync(url, requestcontent).Result;
                return RedirectToAction("List");
            }
            else if (response.IsSuccessStatusCode)
            {
                //No image upload, but update still successful
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

    // GET: Aircraft/Delete/5
    [Authorize]
    public ActionResult DeleteConfirm(int id)
        {
            string url = "aircraftdata/findaircraft/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            AircraftDto selectedaircraft = response.Content.ReadAsAsync<AircraftDto>().Result;
            return View(selectedaircraft);
        }

        // POST: Aircraft/Delete/5
        [HttpPost]
    [Authorize]
    public ActionResult Delete(int id)
        {
        GetApplicationCookie();//get token credentials
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


