using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject model)
        {
            _context.CelestialObjects.Add(model);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new { id = model.Id }, model);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestialObjects = _context.CelestialObjects.Where(x => x.Id == id || x.OrbitedObjectId == id);

            if (celestialObjects.Any() == false)
            {
                return NotFound();
            }

            _context.CelestialObjects.RemoveRange(celestialObjects);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects;

            foreach (var celestialObj in celestialObjects)
            {
                celestialObj.Satellites = GetSatellites(celestialObj.Id);
            }

            return Ok(celestialObjects);
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObj = _context.CelestialObjects.Find(id);

            if (celestialObj == null)
            {
                return NotFound();
            }

            celestialObj.Satellites = GetSatellites(id);

            return Ok(celestialObj);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(x => x.Name == name);

            if (celestialObjects.Any())
            {
                foreach (var celestialObject in celestialObjects)
                {
                    celestialObject.Satellites = GetSatellites(celestialObject.Id);
                }

                return Ok(celestialObjects);
            }

            return NotFound();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var celestialObj = _context.CelestialObjects.Find(id);

            if (celestialObj == null)
            {
                return NotFound();
            }

            celestialObj.Name = name;
            _context.CelestialObjects.Update(celestialObj);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject model)
        {
            var celestialObj = _context.CelestialObjects.Find(id);

            if (celestialObj == null)
            {
                return NotFound();
            }

            celestialObj.Name = model.Name;
            celestialObj.OrbitalPeriod = model.OrbitalPeriod;
            celestialObj.OrbitedObjectId = model.OrbitedObjectId;
            _context.CelestialObjects.Update(celestialObj);
            _context.SaveChanges();

            return NoContent();
        }

        private List<CelestialObject> GetSatellites(int id)
        {
            return _context
                .CelestialObjects.Where(x => x.OrbitedObjectId == id)
                .ToList();
        }
    }
}
