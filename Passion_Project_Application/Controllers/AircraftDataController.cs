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
    public class AircraftDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        /// <summary>
        /// Returns all aircrafts in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all aircrafts in the database, including their associated manufacturer.
        /// </returns>
        /// <example>
        /// GET: api/AircraftData/ListAircrafts
        /// </example>
      
        [HttpGet]
        [ResponseType(typeof(AircraftDto))]
        public IHttpActionResult ListAircrafts()
        {
           List<Aircraft> Aircrafts =  db.Aircrafts.ToList();
            List<AircraftDto> AircraftDtos = new List<AircraftDto>();

            Aircrafts.ForEach(a => AircraftDtos.Add(new AircraftDto()
            {
                AircraftID = a.AircraftID,
                AircraftName = a.AircraftName,
                AircraftType = a.AircraftType,
                YearIntroduced = a.YearIntroduced,
                MaxSpeed = a.MaxSpeed,
                Range = a.Range,
                Description = a.Description,
                ManufacturerID = a.Manufacturers.ManufacturerID,
                CompanyName = a.Manufacturers.CompanyName

            })); 

           return Ok(AircraftDtos);
        }

        /// <summary>
        /// Gathers information about all aircrafts related to a particular manufacturerID
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all aircrafts in the database, including their associated manufaturer matched with a particular manufacturer ID
        /// </returns>
        /// <param name="id">Manufacturer ID.</param>
        /// <example>
        /// GET: api/AircraftData/ListAircraftsForManufacturer/3
        /// </example>
        [HttpGet]
        [ResponseType(typeof(AircraftDto))]
        public IHttpActionResult ListAircraftsForManufacturer(int id)
        {
            List<Aircraft> Aircrafts = db.Aircrafts.Where(a => a.ManufacturerID == id).ToList();
            List<AircraftDto> AircraftDtos = new List<AircraftDto>();

            Aircrafts.ForEach(a => AircraftDtos.Add(new AircraftDto()
            {
                AircraftID = a.AircraftID,
                AircraftName = a.AircraftName,
                AircraftType = a.AircraftType,
                YearIntroduced = a.YearIntroduced,
                MaxSpeed = a.MaxSpeed,
                Range = a.Range,
                Engine = a.Engine,
                Description = a.Description,
                ManufacturerID =a.Manufacturers.ManufacturerID,
                CompanyName = a.Manufacturers.CompanyName
            }));

            return Ok(AircraftDtos);
        }

        /// <summary>
        /// Gathers information about Aircrafts related to a particular Country
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all Aircrafts in the database, including their associated Manufactuer that match to a particular Country id
        /// </returns>
        /// <param name="id">Country ID.</param>
        /// <example>
        /// GET: api/AircraftData/ListAircraftsForCountry/1
        /// </example>
        [HttpGet]
        [ResponseType(typeof(AircraftDto))]
        public IHttpActionResult ListAircraftsForCountry(int id)
        {
            //all aircrafts that operate by countries which match with our ID
            List<Aircraft> Aircrafts = db.Aircrafts.Where(
                a => a.Countries.Any(
                    k => k.CountryID == id
                )).ToList();
            List<AircraftDto> AircraftDtos = new List<AircraftDto>();

            Aircrafts.ForEach(a => AircraftDtos.Add(new AircraftDto()
            {
                AircraftID = a.AircraftID,
                AircraftName = a.AircraftName,
                AircraftType = a.AircraftType,
                YearIntroduced = a.YearIntroduced,
                MaxSpeed = a.MaxSpeed,
                Range = a.Range,
                Engine = a.Engine,
                Description = a.Description,
                ManufacturerID = a.Manufacturers.ManufacturerID,
                CompanyName = a.Manufacturers.CompanyName
            }));

            return Ok(AircraftDtos);
        }



        /// <summary>
        /// Associates a particular country with a particularaircraft
        /// </summary>
        /// <param name="aircraftid">The aircraft ID primary key</param>
        /// <param name="countryid">The country ID primary key</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST api/AircraftData/AssociateAircraftWithCountry/9/1
        /// </example>
        [HttpPost]
        [Route("api/AircraftData/AssociateAircraftWithCountry/{aircraftid}/{countryid}")]
        public IHttpActionResult AssociateAircraftWithCountry(int aircraftid, int countryid)
        {
            
            Aircraft SelectedAircraft = db.Aircrafts.Include(a=>a.Countries).Where(a=>a.AircraftID==aircraftid).FirstOrDefault();
            Country SelectedCountry = db.Countries.Find(countryid);

            if(SelectedAircraft==null || SelectedCountry == null)
            {
                return NotFound();
            }

            Debug.WriteLine("input aircraft id is: " + aircraftid);
            Debug.WriteLine("selected aircraft name is: "+ SelectedAircraft.AircraftName);
            Debug.WriteLine("input country id is: " + countryid);
            Debug.WriteLine("selected country name is: " + SelectedCountry.CountryName);


            SelectedAircraft.Countries.Add(SelectedCountry);
            db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Removes an association between a particular country and a particular aircraft
        /// </summary>
        /// <param name="aircraftid">The aircaft ID primary key</param>
        /// <param name="countryid">Thecountry ID primary key</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST api/AircraftData/UAssociateAircraftWithCountry/9/1
        /// </example>
        [HttpPost]
        [Route("api/AircraftData/UnAssociateAircraftWithCountry/{aircraftid}/{countryid}")]
        public IHttpActionResult UnAssociateAircraftWithCountry(int aircraftid, int countryid)
        {

            Aircraft SelectedAircraft = db.Aircrafts.Include(a => a.Countries).Where(a => a.AircraftID == aircraftid).FirstOrDefault();
            Country SelectedCountry = db.Countries.Find(countryid);

            if (SelectedAircraft == null || SelectedCountry == null)
            {
                return NotFound();
            }

            Debug.WriteLine("input aircraft id is: " + aircraftid);
            Debug.WriteLine("selected aircraft name is: " + SelectedAircraft.AircraftName);
            Debug.WriteLine("input country id is: " + countryid);
            Debug.WriteLine("selected country name is: " + SelectedCountry.CountryName);


            SelectedAircraft.Countries.Remove(SelectedCountry);
            db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Returns all Aircrafts in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: An AIRCRAFT in the system matching up to the aircraft ID primary key
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <param name="id">The primary key of the aircraft</param>
        /// <example>
        /// GET: api/AircraftData/FindAircraft/5
        /// </example>
        [ResponseType(typeof(AircraftDto))]
        [HttpGet]
        public IHttpActionResult FindAircraft(int id)
        {
            Aircraft Aircraft = db.Aircrafts.Find(id);
            AircraftDto AircraftDto = new AircraftDto()
            {
                AircraftID = Aircraft.AircraftID,
                AircraftName = Aircraft.AircraftName,
                AircraftType = Aircraft.AircraftType,
                YearIntroduced = Aircraft.YearIntroduced,
                MaxSpeed = Aircraft.MaxSpeed,
                Range = Aircraft.Range,
                Engine = Aircraft.Engine,
                Description = Aircraft.Description,
                ManufacturerID = Aircraft.Manufacturers.ManufacturerID,
                CompanyName = Aircraft.Manufacturers.CompanyName
            };
            if (Aircraft == null)
            {
                return NotFound();
            }

            return Ok(AircraftDto);
        }

        /// <summary>
        /// Updates a particular aircraft in the system with POST Data input
        /// </summary>
        /// <param name="id">Represents the Aircraft ID primary key</param>
        /// <param name="aircraft">JSON FORM DATA of an aircraft</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/AircraftData/UpdatAircraft/5
        /// FORM DATA: Aircraft JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateAircraft(int id, Aircraft aircraft)
        { 
            if (!ModelState.IsValid)
            {      
                return BadRequest(ModelState);
            }

            if (id != aircraft.AircraftID)
            {
                
                return BadRequest();
            }

            db.Entry(aircraft).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AircraftExists(id))
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
        /// Adds an aircraft to the system
        /// </summary>
        /// <param name="aircraft">JSON FORM DATA of an aircraft</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: Aircraft ID, Aircraft Data
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/AircraftData/AddAircraft
        /// FORM DATA: Aircraft JSON Object
        /// </example>
        [ResponseType(typeof(Aircraft))]
        [HttpPost]
        public IHttpActionResult AddAircraft(Aircraft aircraft)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Aircrafts.Add(aircraft);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = aircraft.AircraftID}, aircraft);
        }

        /// <summary>
        /// Deletes an aircraft from the system by it's ID.
        /// </summary>
        /// <param name="id">The primary key of the aircarft</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST: api/AircraftData/DeleteAircraft/5
        /// FORM DATA: (empty)
        /// </example>
        [ResponseType(typeof(Aircraft))]
        [HttpPost]
        public IHttpActionResult DeleteAircraft(int id)
        {
            Aircraft aircraft = db.Aircrafts.Find(id);
            if (aircraft == null)
            {
                return NotFound();
            }

            db.Aircrafts.Remove(aircraft);
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

        private bool AircraftExists(int id)
        {
            return db.Aircrafts.Count(e => e.AircraftID == id) > 0;
        }
    }
}

       