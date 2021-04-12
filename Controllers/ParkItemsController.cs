using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using leashApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using AutoMapper;
using leashed.Controllers.Resources;

namespace leashApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("TestApp")] 
    public class ParkItemsController : ControllerBase
    {
        private readonly ParkContext _context;
        private readonly IMapper _mapper;

        public ParkItemsController(ParkContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/ParkItems
        [HttpGet("All")]
        public async Task<ActionResult<IEnumerable<ParkItemResource>>> GetParkItems()
        {
            Console.Out.WriteLine("Get works");
            Console.Error.WriteLine("get works");
            Console.Out.Flush();
            var result = await _context.ParkItems.Include(p => p.ParkGoers).ToListAsync();
            
            
            return Ok(_mapper.Map<IList<ParkItem>,IEnumerable<ParkItemResource>>(result));
        }


        // GET: api/ParkItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ParkItemResource>> GetParkItem(long id)
        {
            var parkItem = await _context.ParkItems.FindAsync(id);

            if (parkItem == null)
            {
                return NotFound();
            }

            await _context.Entry(parkItem).Collection(p => p.ParkGoers).LoadAsync();

            return Ok(_mapper.Map<ParkItem,ParkItemResource>(parkItem));
        }

        // GET: api/ParkItems/Name/"park name"
        [HttpGet("Name/{name}")]
        public async Task<ActionResult<ParkItemResource>> GetParkItemByName(string name)
        {
            var parkItem = await _context.ParkItems.Include(p => p.ParkGoers).FirstOrDefaultAsync(x => x.Name == name);

            if (parkItem == null)
            {
                return NotFound();
            }
            await _context.Entry(parkItem).Collection(p => p.ParkGoers).LoadAsync();

            return Ok(_mapper.Map<ParkItem,ParkItemResource>(parkItem));
        }

        // GET: api/ParkItems/suburb/city+name
        //Get list of parks by suburb
        [HttpGet("suburb/{suburb}")]
        public async Task<ActionResult<IEnumerable<ParkItemResource>>> GetParkSuburbItem(String suburb)
        {
            var result = await _context.ParkItems.Where( li => 
                li.Suburb == suburb
            ).Include(p => p.ParkGoers).ToListAsync();
         
            if (result.Count == 0)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<IList<ParkItem>,IEnumerable<ParkItemResource>>(result));

        }

        // PUT: api/ParkItems/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutParkItem(long id, ParkItemResource parkItemResource)
        {   

            var parkItem = _mapper.Map<ParkItemResource, ParkItem>(parkItemResource);

            if (id != parkItem.Id)
            {
                return BadRequest();
            }



            _context.Entry(parkItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ParkItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ParkItems
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ParkItemResource>> PostParkItem(ParkItemResource parkItemResource)
        {
            //var parkItem = _mapper.Map<ParkItemResource, ParkItem>(parkItemResource);
            var parkItem = new ParkItem{
                Name = parkItemResource.Name,
                IsLeashed = parkItemResource.IsLeashed,
                RoadFront = parkItemResource.RoadFront,
                Suburb = parkItemResource.Suburb,
                City = parkItemResource.City,
                Country = parkItemResource.Country,
                ParkGoers = _mapper.Map<IList<UserResource>,IList<UserData>>(parkItemResource.ParkGoers),

            };

            _context.ParkItems.Add(parkItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetParkItem", new { id = parkItem.Id }, _mapper.Map<ParkItem,ParkItemResource>(parkItem));
        }

        // DELETE: api/ParkItems/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "IsAdmin")]
        public async Task<ActionResult<ParkItemResource>> DeleteParkItem(long id)
        {
            var parkItem = await _context.ParkItems.FindAsync(id);
            if (parkItem == null)
            {
                return NotFound();
            }

            _context.ParkItems.Remove(parkItem);
            await _context.SaveChangesAsync();

            return Ok(_mapper.Map<ParkItem,ParkItemResource>(parkItem));
        }

        private bool ParkItemExists(long id)
        {
            return _context.ParkItems.Any(e => e.Id == id);
        }
    }
}
