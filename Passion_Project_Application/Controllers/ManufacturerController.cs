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



        // GET: Manufacturer/List
        public ActionResult List(string search)

        {
            //objective: communicate with our Manufacturer data api to retrieve a list fo Manufacturers
            //curl https://localhost:44384/api/manufacturerdata/ListManufacturers

            string url = "manufacturerdata/listmanufacturers";
            HttpResponseMessage response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {


                IEnumerable<ManufacturerDto> SelectedManufacturer = response.Content.ReadAsAsync<IEnumerable<ManufacturerDto>>().Result;
                return View(search == null ? SelectedManufacturer :
                    SelectedManufacturer.Where(x => x.CompanyName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0).ToList());
                //Debug.WriteLine("The response code is ");
                //Debug.WriteLine(response.StatusCode);
                //Debug.WriteLine("Number of Manufacturer received : ");
                //Debug.WriteLine(Manufactrer.Count());
            }
            else
            {
                return RedirectToAction("Error");
            }
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
        [Authorize]
        public ActionResult Create(Manufacturer manufacturer)
        {
            GetApplicationCookie();//get token credentials
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
        [Authorize]
        public ActionResult Edit(int id)
        {

            string url = "manufacturerdata/findmanufacturer/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            ManufacturerDto selectedManufacturer = response.Content.ReadAsAsync<ManufacturerDto>().Result;
            return View(selectedManufacturer);
        }

        // POST: Manufacturer/Update/5
        [HttpPost]
        [Authorize]
        public ActionResult Update(int id, Manufacturer Manufacturer , HttpPostedFileBase ManufacturerPic)
        {
            GetApplicationCookie();//get token credentials
            string url = "manufacturerdata/updatemanufacturer/" + id;
            string jsonpayload = jss.Serialize(Manufacturer);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            Debug.WriteLine(content);
            if (response.IsSuccessStatusCode && ManufacturerPic !=null)
            {
                //Send over image data for Manufacturer
                url = "ManufacturerData/UploadManufacturerPic/" + id;
                //Debug.WriteLine("Received ManufacturerPicture "+ManufacturerPic.FileName);

                MultipartFormDataContent requestcontent = new MultipartFormDataContent();
                HttpContent imagecontent = new StreamContent(ManufacturerPic.InputStream);
                requestcontent.Add(imagecontent, "ManufacturerPic", ManufacturerPic.FileName);
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

        // GET: Manufacturer/DeleteConfirm/5
        [Authorize]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "manufacturerdata/findmanufacturer/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            ManufacturerDto selectedManufacturer = response.Content.ReadAsAsync<ManufacturerDto>().Result;
            return View(selectedManufacturer);
        }

        // POST: Manufacturer/Delete/5
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();//get token credentials
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
