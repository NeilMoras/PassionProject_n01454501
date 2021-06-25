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
    public class CountryController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static CountryController()
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
        /// Authentication cookie is grabbed which was sent to the controller
        /// Provides 
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

        // GET: Country/List
        [HttpGet]
        public ActionResult List(string search)
        {
            //objective: communicate with our Country data api to retrieve a list fo Countries
            //curl https://localhost:44384/api/countrydata/ListCountries

            string url = "countrydata/listcountries";
            HttpResponseMessage response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {


                IEnumerable<CountryDto> SelectedCountry = response.Content.ReadAsAsync<IEnumerable<CountryDto>>().Result;
                return View(search == null ? SelectedCountry :
                    SelectedCountry.Where(x => x.CountryName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0).ToList());
                //Debug.WriteLine("The response code is ");
                //Debug.WriteLine(response.StatusCode);


                //Debug.WriteLine("Number of Countries received : ");
                //Debug.WriteLine(Country.Count());

                // return View(Countries);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
    

    

        

        // GET: Country/Details/5
        [HttpGet]
        public ActionResult Details(int id)
        {
            DetailsCountry ViewModel = new DetailsCountry();

            //objective: communicate with our Country data api to retrieve one Country
            //curl https://localhost:44324/api/Countrydata/findcountry/{id}

            string url = "countrydata/findcountry/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            Debug.WriteLine("The response code is ");
            Debug.WriteLine(response.StatusCode);

            CountryDto SelectedCountry = response.Content.ReadAsAsync<CountryDto>().Result;
            Debug.WriteLine("Country received : ");
            Debug.WriteLine(SelectedCountry.CountryName);

            ViewModel.SelectedCountry = SelectedCountry;

            //show all Aircrafts under operatiin of this country
            url = "aircraftdata/listaircraftsforcountry/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<AircraftDto> KeptAircrafts = response.Content.ReadAsAsync<IEnumerable<AircraftDto>>().Result;

            ViewModel.KeptAircrafts = KeptAircrafts;


            return View(ViewModel);
        }
        public ActionResult Error()
        {
            return View();
        }


        // GET: Country/New
        [HttpGet]
        [Authorize]
        public ActionResult New()
        {
            return View();
        }

        // POST: Country/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(Country country)
        {

            GetApplicationCookie();//get token credentials
            Debug.WriteLine("the json payload is :");
            //Debug.WriteLine(Country.CompanyName);
            //objective: add a new Manufacturer into our system using the API
            //curl -H "Content-Type:application/json" -d @Manufacturer.json https://localhost:44324/api/Manufacturerdata/addmanufacturer 
            string url = "countrydata/addcountry";


            string jsonpayload = jss.Serialize(country);
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

        // GET: Country/Edit/5
        [HttpGet]
        [Authorize]
        public ActionResult Edit(int id)
        {
           
            string url = "countrydata/findcountry/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            CountryDto selectedCountry = response.Content.ReadAsAsync<CountryDto>().Result;
            return View(selectedCountry);
        }

        // POST: Country/Update/5
        [HttpPost]
        [Authorize]
        public ActionResult Update(int id, Country Country)
        {
            GetApplicationCookie();//get token credentials
            string url = "countrydata/updatecountry/" + id;
            string jsonpayload = jss.Serialize(Country);
            HttpContent content = new StringContent(jsonpayload);
            Debug.WriteLine("Json payload is :");
            Debug.WriteLine(jsonpayload);
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

        // GET: Country/DeleteConfirm/5
        [Authorize]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "countrydata/findcountry/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            CountryDto selectedCountry = response.Content.ReadAsAsync<CountryDto>().Result;
            return View(selectedCountry);
        }

        // POST: Country/Delete/5
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();//get token credentials
            string url = "countrydata/deletecountry/" + id;
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
