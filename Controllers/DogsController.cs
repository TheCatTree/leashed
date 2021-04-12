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
using AutoMapper;
using leashed.Services;

namespace leashApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("TestApp")] 
    public class DogsController : Controller
    {
        private readonly ParkContext _context;
        private readonly IMapper _mapper;
        private readonly ITokenUserResolverService _user;

        public DogsController(ParkContext context, IMapper mapper, ITokenUserResolverService user)
        {
            _context = context;
            _mapper = mapper;
            _user = user;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> getDog(int id){

            var dog = await _context.Dogs.FindAsync(id);
           if (dog == null)
           {
                return StatusCode(403, $"No User Data: {id} ");
            }


            return Ok(_mapper.Map<Dog,DogResource>(dog));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ParkItemResource>> deleteDog(int id)
        {
            var dog = await _context.Dogs.FindAsync(id);
            if (dog == null)
            {
                return NotFound();
            }
            var owner = await _context.UserData.FindAsync(dog.UserDataId);
            
            if(!_user.isAdmin() && ( owner==null || _user.getSub() != owner.TokenSub.tokenSub)){
                return StatusCode(403, $"Not Authorized to modify this dog");
            }


            _context.Dogs.Remove(dog);
            await _context.SaveChangesAsync();

            return Ok(_mapper.Map<Dog,DogResource>(dog));
        }

        [HttpGet("/user/{id}/dogs")]
        public async Task<ActionResult<IEnumerable<DogResource>>> GetUsersDogs(int id){

            var dogs = await _context.Dogs.Where( dog => 
                dog.UserDataId == id
            ).ToListAsync();
            Console.WriteLine("---------------------------------------------------------------------------------------------------------");
            Console.WriteLine(dogs.Count());
            Console.WriteLine("---------------------------------------------------------------------------------------------------------");
            return Ok(_mapper.Map<List<Dog>,IList<DogResource>>(dogs));
        }
       

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<DogResource>> Postdog([FromBody] DogResource dogData)
        {
            Console.WriteLine("Dog post method.");
            if (!ModelState.IsValid){
                 Console.WriteLine("Invalid model state.");
                 return BadRequest(ModelState);

            }
               
            
            var user = await _context.UserData.FindAsync(dogData.UserDataId);
            var dog = new Dog{
             UserDataId = user.Id,
                Name = dogData.Name
            };
             
            _context.Dogs.Add(dog);
            await _context.SaveChangesAsync();
            dog = await _context.Dogs.FindAsync(dog.Id);
            return Ok(_mapper.Map<Dog,DogResource>(dog));
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult<Dog>> udateDog([FromBody] DogResource dogData)
        {
            var id = dogData.Id;
            var user = await _context.UserData.Where(x => x.TokenSub.tokenSub == _user.getSub()).FirstOrDefaultAsync();
            if (user == null && !_user.isAdmin())
            {
                return StatusCode(403, $"No editor Data");
            }
            var dog = await _context.Dogs.FindAsync(id);
            if (dog == null)
            {
                return StatusCode(403, $"No dog to modify");
            }

            if(_user.isAdmin() || user.Id == dog.UserDataId){
                dog.Name = dogData.Name;
                dog.UserDataId = dogData.UserDataId;
                _context.Entry(dog).State = EntityState.Modified;
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.UserData.Any(e => e.Id == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                   
                
                dog = await _context.Dogs.FindAsync(id);
                return Ok(_mapper.Map<Dog,DogResource>(dog));
            }

            return NotFound();
        }

     
    }
}