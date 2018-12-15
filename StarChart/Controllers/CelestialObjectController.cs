using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            if (celestialObj != null)
            {
                celestialObj.Satellites = GetSatellites(id);

                return Ok(celestialObj);
            }

            return NotFound();
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(x => x.Name == name);

            if (celestialObjects.Any())
            {
                foreach(var celestialObject in celestialObjects)
                {
                    celestialObject.Satellites = GetSatellites(celestialObject.Id);
                }

                return Ok(celestialObjects);
            }

            return NotFound();
        }

        private List<CelestialObject> GetSatellites(int id)
        {
            return _context
                .CelestialObjects.Where(x => x.OrbitedObjectId == id)
                .ToList();
        }
    }
}
