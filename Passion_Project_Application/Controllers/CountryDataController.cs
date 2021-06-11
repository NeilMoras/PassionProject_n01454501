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
    public class CountryDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Returns all the Countries in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all Countries in the database
        /// </returns>
        /// <example>
        /// GET: api/CountryData/ListCountries
        /// </example>

        [HttpGet]
        [ResponseType(typeof(CountryDto))]
        public IHttpActionResult ListCountries()
        {
            List<Country> Countries = db.Countries.ToList();
            List<CountryDto> CountryDtos = new List<CountryDto>();

            Countries.ForEach(k => CountryDtos.Add(new CountryDto()
            {
                CountryID = k.CountryID,
                CountryName = k.CountryName,
                AirforceName = k.AirforceName,
                CountryDescription = k.CountryDescription
            }));
            return Ok(CountryDtos);
        }
        /// <summary>
        /// Returns all Countries in the system associated with a particular Aircraft.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all Countriesi the database owning and operating of a particular Aircraft
        /// </returns>
        /// <param name="id">Aircraft Primary Key</param>
        /// <example>
        /// GET: api/CountryData/ListCountriesForAircraft/1
        /// </example>
        [HttpGet]
        [ResponseType(typeof(Country))]
        public IHttpActionResult ListCountriesForAircraft(int id)
        {
            List<Country> Countries = db.Countries.Where(
                k => k.Aircrafts.Any(
                    a => a.AircraftID == id)
                ).ToList();
            List<CountryDto> CountryDtos = new List<CountryDto>();

           Countries.ForEach(k =>CountryDtos.Add(new CountryDto()
            {
               CountryID = k.CountryID,
               CountryName = k.CountryName,
               AirforceName = k.AirforceName,
               CountryDescription = k.CountryDescription
           }));

            return Ok(CountryDtos);
        }

        /// <summary>
        /// Returns Countries in the system not caring for a particular Aircraft.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all Countries in the database not taking care of a particular Aircraft
        /// </returns>
        /// <param name="id">Aircraft Primary Key</param>
        /// <example>
        /// GET: api/CountryData/ListCountriesNotOperatingAircraft/1
        /// </example>
        [HttpGet]
        [ResponseType(typeof(CountryDto))]
        public IHttpActionResult ListCountriesNotOperatingAircraft(int id)
        {
            List<Country> Countries = db.Countries.Where(
                k => !k.Aircrafts.Any(
                    a => a.AircraftID == id)
                ).ToList();
            List<CountryDto> CountryDtos = new List<CountryDto>();

            Countries.ForEach(k => CountryDtos.Add(new CountryDto()
            {
                CountryID = k.CountryID,
                CountryName = k.CountryName,
                AirforceName = k.AirforceName,
                CountryDescription = k.CountryDescription
            }));

            return Ok(CountryDtos);
        }


        /// <summary>
        /// Returns all Countries in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: An Country in the system matching up to the Country ID primary key
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <param name="id">The primary key of the Country</param>
        /// <example>
        /// GET: api/CountryData/FindCountry/5
        /// </example>
        [ResponseType(typeof(CountryDto))]
        [HttpGet]
        public IHttpActionResult FindCountry(int id)
        {
            Country Country = db.Countries.Find(id);
            CountryDto CountryDto = new CountryDto()
            {
                CountryID = Country.CountryID,
                CountryName = Country.CountryName,
                AirforceName = Country.AirforceName,
                CountryDescription =Country.CountryDescription
            };
            if (Country == null)
            {
                return NotFound();
            }

            return Ok(CountryDto);
        }

        /// <summary>
        /// Updates a particular Country in the system with POST Data input
        /// </summary>
        /// <param name="id">Represents the Country ID primary key</param>
        /// <param name="Country">JSON FORM DATA of an Country</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/CountryData/UpdateCountry/5
        /// FORM DATA:Country JSON Object
        /// </example>

        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateCountry(int id, Country Country)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != Country.CountryID)
            {
                return BadRequest();
            }

            db.Entry(Country).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CountryExists(id))
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
        /// Adds an Country to the system
        /// </summary>
        /// <param name="Country">JSON FORM DATA of a Country</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: Country ID, Country Data
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/CountryData/AddCountry
        /// FORM DATA:Country JSON Object
        /// </example>

        
        [ResponseType(typeof(Country))]
        [HttpPost]
        public IHttpActionResult AddCountry(Country country)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Countries.Add(country);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = country.CountryID }, country);
        }

        /// <summary>
        /// Deletes a Country from the system by it's ID.
        /// </summary>
        /// <param name="id">The primary key of a Country</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST: api/CountryData/DeleteCountry/5
        /// FORM DATA: (empty)
        /// </example>
        
        [ResponseType(typeof(Country))]
        [HttpPost]
        public IHttpActionResult DeleteCountry(int id)
        {
            Country country = db.Countries.Find(id);
            if (country == null)
            {
                return NotFound();
            }

            db.Countries.Remove(country);
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

        private bool CountryExists(int id)
        {
            return db.Countries.Count(e => e.CountryID == id) > 0;
        }
    }
}