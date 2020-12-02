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
using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Diagnostics;

namespace leashApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("TestApp")] 
    public class DogsController : Controller
    {
        private readonly ParkContext _context;

        public DogsController(ParkContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDog(String id){

            var dog = await _context.Dogs.FindAsync(id);
           if (dog == null)
           {
                return StatusCode(403, $"No User Data: {id} ");
            }


            return Ok(dog);
        }

        [HttpGet("/user/{id}/dogs")]
        public async Task<ActionResult<IEnumerable<Dog>>> GetUsersDogs(int id){

            return await _context.Dogs.Where( dog => 
                dog.Id == id
            ).ToListAsync();
        }
       

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Dog>> Postdog([FromBody] Dog dogData)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Dogs.Add(dogData);
            await _context.SaveChangesAsync();
            var id = dogData.Id;
            var dog = await _context.Dogs.FindAsync(id);
            return Ok(dog);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<Dog>> UdateDog(int id, [FromBody] Dog dogData)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var dog = await _context.Dogs.FindAsync(id);

            if (dog == null){
                return NotFound();
            }
            dogData.Id = dog.Id;
            dog = dogData;
            await _context.SaveChangesAsync();
            
            
            dog = await _context.Dogs.FindAsync(id);
            return Ok(dog);
        }

     
    }
}