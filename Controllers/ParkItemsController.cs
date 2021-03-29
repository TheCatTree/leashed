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

namespace leashApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("TestApp")] 
    public class ParkItemsController : ControllerBase
    {
        private readonly ParkContext _context;

        public ParkItemsController(ParkContext context)
        {
            _context = context;
        }

        // GET: api/ParkItems
        [HttpGet("suburb")]
        public async Task<ActionResult<IEnumerable<ParkItem>>> GetParkItems()
        {
            Console.Out.WriteLine("Get works");
            Console.Error.WriteLine("get works");
            Console.Out.Flush();
            return await _context.ParkItems.ToListAsync();
        }

        // GET: api/ParkItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ParkItem>> GetParkItem(long id)
        {
            var parkItem = await _context.ParkItems.FindAsync(id);

            if (parkItem == null)
            {
                return NotFound();
            }

            return parkItem;
        }

        // GET: api/ParkItems/Name/"park name"
        [HttpGet("Name/{name}")]
        public async Task<ActionResult<ParkItem>> GetParkItemByName(string name)
        {
            var parkItem = await _context.ParkItems.FirstOrDefaultAsync(x => x.Name == name);

            if (parkItem == null)
            {
                return NotFound();
            }

            return parkItem;
        }

        // GET: api/ParkItems/suburb/city+name
        //Get list of parks by suburb
        [HttpGet("suburb/{suburb}")]
        public async Task<ActionResult<IEnumerable<ParkItem>>> GetParkSuburbItem(String suburb)
        {
            var parkItem = await _context.ParkItems.Where( li => 
                li.Suburb == suburb
            ).ToListAsync();
         
            if (parkItem.Count == 0)
            {
                return NotFound();
            }

            return parkItem;

        }

        // PUT: api/ParkItems/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutParkItem(long id, ParkItem parkItem)
        {
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
        public async Task<ActionResult<ParkItem>> PostParkItem(ParkItem parkItem)
        {
            _context.ParkItems.Add(parkItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetParkItem", new { id = parkItem.Id }, parkItem);
        }

        // DELETE: api/ParkItems/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "IsAdmin")]
        public async Task<ActionResult<ParkItem>> DeleteParkItem(long id)
        {
            var parkItem = await _context.ParkItems.FindAsync(id);
            if (parkItem == null)
            {
                return NotFound();
            }

            _context.ParkItems.Remove(parkItem);
            await _context.SaveChangesAsync();

            return parkItem;
        }

        private bool ParkItemExists(long id)
        {
            return _context.ParkItems.Any(e => e.Id == id);
        }
    }
}
