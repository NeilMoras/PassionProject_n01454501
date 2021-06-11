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
                CompanyDescription = s.CompanyDescription
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
                CompanyDescription = Manufacturer.CompanyDescription
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
        public IHttpActionResult DeleteManufacturer(int id)
        {
            Manufacturer Manufacturer = db.Manufacturers.Find(id);
            if (Manufacturer == null)
            {
                return NotFound();
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