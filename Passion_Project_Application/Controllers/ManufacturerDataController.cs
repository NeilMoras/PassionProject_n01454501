using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Passion_Project_Application.Models;
using System.Diagnostics;
using System.Web;
using System.IO;

namespace Passion_Project_Application.Controllers
{
    public class ManufacturerDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Returns all manufacturers in the system
        /// </summary>
        /// <returns>
        /// HEADER 200(OK)
        /// Content all manufacturer in the database,including their associated apecies
        /// </returns>
        ///<example>
        ///// GET: api/ManufacturerData/ListManufacturer
        ///</example>
        [HttpGet]
        [ResponseType(typeof(ManufacturerDto))]
        public IHttpActionResult ListManufacturers()
        {
            List<Manufacturer> Manufacturers = db.Manufacturers.ToList();
            List<ManufacturerDto> ManufacturerDtos = new List<ManufacturerDto>();

            Manufacturers.ForEach(s => ManufacturerDtos.Add(new ManufacturerDto()
            {
                ManufacturerID = s.ManufacturerID,
                CompanyName = s.CompanyName,
                Country = s.Country,
                 HeadQuarters = s.HeadQuarters,
                CompanyDescription = s.CompanyDescription,
                ManufacturerHasPic = s.ManufacturerHasPic,
                ManufacturerPicExtension = s.ManufacturerPicExtension
            }));
            return Ok(ManufacturerDtos);
        }

        /// <summary>
        /// Returns one Manufacturer in the system
        /// </summary>
        /// <param name="id">The primary key of the manufacturer</param>
        /// <returns>
        /// HEADER 200(OK)
        /// CONTENT: A Manufacturer in the system matching up to the Manufacturer ID primary key
        /// or 
        /// HEADER 404(NOT FOUND)
        /// </returns>
        // <exampple>GET: api/ManufacturerData/FindManufacturer/5</example>
        [ResponseType(typeof(ManufacturerDto))]
        [HttpGet]
        public IHttpActionResult FindManufacturer(int id)
        {
            Manufacturer Manufacturer = db.Manufacturers.Find(id);
            ManufacturerDto ManufacturerDto = new ManufacturerDto()
            {
                ManufacturerID = Manufacturer.ManufacturerID,
                CompanyName = Manufacturer.CompanyName,
                Country = Manufacturer.Country,
                HeadQuarters = Manufacturer.HeadQuarters,
                CompanyDescription = Manufacturer.CompanyDescription,
                ManufacturerHasPic = Manufacturer.ManufacturerHasPic,
                ManufacturerPicExtension = Manufacturer.ManufacturerPicExtension
            };
            if (Manufacturer == null)
            {
                return NotFound();
            }

            return Ok(ManufacturerDto);
        }

        /// <summary>
        /// Updates a particular Maunfacturer in the system with POST Data input
        /// </summary>
        /// <param name="id">Represents the Manufacturer ID primary key</param>
        /// <param name="Manufacturer">JSON FORM DATA of a Manufacturer</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
      /// <example>
             // PUT: api/ManufacturerData/UpdateManufacturer/5
             //FROM DATA: Manufacuter JSON Object 
          ///  </example>
          ///  
        [ResponseType(typeof(void))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult UpdateManufacturer(int id, Manufacturer Manufacturer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != Manufacturer.ManufacturerID)
            {
                return BadRequest();
            }

            db.Entry(Manufacturer).State = EntityState.Modified;
            // Picture update is handled by another method
            db.Entry(Manufacturer).Property(a => a.ManufacturerHasPic).IsModified = false;
            db.Entry(Manufacturer).Property(a => a.ManufacturerPicExtension).IsModified = false;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ManufacturerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Receives Manufacturer picture data, uploads it to the webserver and updates the Manufacturer's HasPic option
        /// </summary>
        /// <param name="id">themanufacturer id</param>
        /// <returns>status code 200 if successful.</returns>
        /// <example>
        /// curl -F manufactuerpic=@file.jpg "https://localhost:xx/api/manufacturerdata/uploadmanufacturerpic/2"
        /// POST: api/manufacturerData/UpdatemanufacturerPic/3
        /// HEADER: enctype=multipart/form-data
        /// FORM-DATA: image
        /// </example>

        [HttpPost]
        public IHttpActionResult UploadManufacturerPic(int id)
        {

            bool haspic = false;
            string picextension;
            if (Request.Content.IsMimeMultipartContent())
            {
                Debug.WriteLine("Received multipart form data.");

                int numfiles = HttpContext.Current.Request.Files.Count;
                Debug.WriteLine("Files Received: " + numfiles);

                //Check if a file is posted
                if (numfiles == 1 && HttpContext.Current.Request.Files[0] != null)
                {
                    var manufacturerPic = HttpContext.Current.Request.Files[0];
                    //Check if the file is empty
                    if (manufacturerPic.ContentLength > 0)
                    {
                        //establish valid file types (can be changed to other file extensions if desired!)
                        var valtypes = new[] { "jpeg", "jpg", "png", "gif" };
                        var extension = Path.GetExtension(manufacturerPic.FileName).Substring(1);
                        //Check the extension of the file
                        if (valtypes.Contains(extension))
                        {
                            try
                            {
                                //file name is the id of the image
                                string fn = id + "." + extension;

                                //get a direct file path to ~/Content/aircrafts/{id}.{extension}
                                string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/Images/Manufacturers/"), fn);

                                //save the file
                                manufacturerPic.SaveAs(path);

                                //if these are all successful then we can set these fields
                                haspic = true;
                                picextension = extension;

                                //Update the animal haspic and picextension fields in the database
                                Manufacturer SelectedManufacturer = db.Manufacturers.Find(id);
                                SelectedManufacturer.ManufacturerHasPic = haspic;
                                SelectedManufacturer.ManufacturerPicExtension = extension;
                                db.Entry(SelectedManufacturer).State = EntityState.Modified;

                                db.SaveChanges();

                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("Manufacturer Image was not saved successfully.");
                                Debug.WriteLine("Exception:" + ex);
                                return BadRequest();
                            }
                        }
                    }

                }

                return Ok();
            }
            else
            {
                //not multipart form data
                return BadRequest();

            }

        }

        /// <summary>
        /// Adds a Manufacturer to the system
        /// </summary>
        /// <param name="Manufacturer"> JSON FORM DATA of the manufacturer</param>
        /// <returns>
        ///  /// HEADER: 201 (Created)
        /// CONTENT: Manufacturer ID, Manufacturer Data
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// // POST: api/ManufacturerData/AddManufacturer
        /// </example>

        [ResponseType(typeof(Manufacturer))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult AddManufacturer(Manufacturer Manufacturer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Manufacturers.Add(Manufacturer);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = Manufacturer.ManufacturerID }, Manufacturer);
        }

        /// <summary>
        /// Deletes a Manufacturer from the system by it's ID.
        /// </summary>
        /// <param name="id">The primary kay of the Maunfacturer</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        // DELETE: api/ManufacturerData/DeleteManufacturer/5
        ///FORM DATA: (empty)
        /// </example>


        [ResponseType(typeof(Manufacturer))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult DeleteManufacturer(int id)
        {
            Manufacturer Manufacturer = db.Manufacturers.Find(id);
            if (Manufacturer == null)
            {
                return NotFound();
            }
            if (Manufacturer.ManufacturerHasPic && Manufacturer.ManufacturerPicExtension != "")
            {
                //also delete image from path
                string path = HttpContext.Current.Server.MapPath("~/Content/Images/Manufacturers/" + id + "." + Manufacturer.ManufacturerPicExtension);
                if (System.IO.File.Exists(path))
                {
                    Debug.WriteLine("File exists... preparing to delete!");
                    System.IO.File.Delete(path);
                }
            }

            db.Manufacturers.Remove(Manufacturer);
            db.SaveChanges();

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ManufacturerExists(int id)
        {
            return db.Manufacturers.Count(e => e.ManufacturerID == id) > 0;
        }
    }
}