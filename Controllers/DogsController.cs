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
using leashed.Controllers.Resources;

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

            var dogs = await _context.Dogs.Where( dog => 
                dog.UserDataId == id
            ).ToListAsync();
            Console.WriteLine("---------------------------------------------------------------------------------------------------------");
            Console.WriteLine(dogs.Count());
            Console.WriteLine("---------------------------------------------------------------------------------------------------------");
            return Ok(dogs);
        }
       

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Dog>> Postdog([FromBody] DogResource dogData)
        {
            Console.WriteLine("Dog post method.");
            if (!ModelState.IsValid){
                 Console.WriteLine("Invalid model state.");
                 return BadRequest(ModelState);

            }
               
            
            var user = await _context.UserData.FindAsync(dogData.UserDataId);
            var dog = new Dog{
             UserDataId = user.Id,
                Name = dogData.name
            };
             
            _context.Dogs.Add(dog);
            await _context.SaveChangesAsync();
            dog = await _context.Dogs.FindAsync(dog.Id);
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