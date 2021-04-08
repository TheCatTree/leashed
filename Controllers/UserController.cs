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
using System.Collections.ObjectModel;
using AutoMapper;


namespace leashApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("TestApp")] 
    public class UserController : Controller
    {
        private readonly ParkContext _context;
        private readonly IMapper _mapper;

        public UserController(ParkContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetUser(){

            var token = await HttpContext.GetTokenAsync("access_token");
            var securityTokenHandler = new JwtSecurityTokenHandler();
            var decriptedToken = securityTokenHandler.ReadJwtToken(token);
            var claims = decriptedToken.Claims;
            var sub = claims.First(c => c.Type == "sub").Value;
            var user = await _context.UserData.Where(x => x.TokenSub.tokenSub == sub).FirstOrDefaultAsync();
            Console.WriteLine("---------------------sub-----------------------");
            Console.WriteLine(sub);
           if (user == null)
           {
                return StatusCode(403, $"No User Data: {sub} ");
            }


            return Ok(_mapper.Map<UserData,UserResource>(user));
        }

        [HttpGet("listusers")]
        public async Task<ActionResult<ICollection<UserResource>>> ListUser(){
            
            var users = await _context.UserData.ToListAsync();
            var usersOut = new Collection<UserResource>();
            foreach(UserData user in users){
                var x = new UserResource();
                x = _mapper.Map<UserData,UserResource>(user);
                usersOut.Add(x);
            }
            return usersOut;
        }

        [HttpGet("friends")]
        public async Task<ActionResult<ICollection<UserResource>>> GetFriends(){

            var token = await HttpContext.GetTokenAsync("access_token");
            var securityTokenHandler = new JwtSecurityTokenHandler();
            var decriptedToken = securityTokenHandler.ReadJwtToken(token);
            var claims = decriptedToken.Claims;
            var sub = claims.First(c => c.Type == "sub").Value;
            var user = await _context.UserData.Where(x => x.TokenSub.tokenSub == sub).FirstOrDefaultAsync();
            Console.WriteLine("---------------------sub-----------------------");
            Console.WriteLine(sub);
           if (user == null)
           {
                return StatusCode(403, $"No User Data: {sub} ");
            }

            var usersOut = new Collection<UserResource>();
            foreach(UserData friend in user.friends){
                var x = new UserResource();
                x = _mapper.Map<UserData,UserResource>(user);
                usersOut.Add(x);
            }

            return usersOut;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(String id){

            var user = await _context.UserData.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }


            return Ok(_mapper.Map<UserData,UserResource>(user));
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<UserResource>> Postuser()
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var securityTokenHandler = new JwtSecurityTokenHandler();
            var decriptedToken = securityTokenHandler.ReadJwtToken(token);
            var claims = decriptedToken.Claims;
            var sub = claims.First(c => c.Type == "sub").Value;
            var user = await _context.UserData.Where(x => x.TokenSub.tokenSub == sub).FirstOrDefaultAsync();
            Console.WriteLine("---------------------sub-----------------------");
            Console.WriteLine(sub);
            Console.WriteLine("---------------------USER FROM POST-----------------------");
            Console.WriteLine(user);
            if (user != null)
            {
                return StatusCode(403, $"User with auth0 sub, already exists: {sub} {user}");
            }

            user = new UserData
            {
                TokenSub = new TokenSub{
                tokenSub =  sub
                },
                Name = "Not Set Yet"
            };

            _context.UserData.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostUserById", new { id = user.Id }, _mapper.Map<UserData, UserResource>(user));
        }

        [HttpPost("addFriend/{id}")]
        public async Task<ActionResult<UserResource>> userAddFriends(int id)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var securityTokenHandler = new JwtSecurityTokenHandler();
            var decriptedToken = securityTokenHandler.ReadJwtToken(token);
            var claims = decriptedToken.Claims;
            var sub = claims.First(c => c.Type == "sub").Value;
            var user = await _context.UserData.Where(x => x.TokenSub.tokenSub == sub).FirstOrDefaultAsync();
            var friend = await _context.UserData.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (user == null)
            {
                return StatusCode(403, $"No user");
            }
            if (friend == null)
            {
                return StatusCode(403, $"No friend");
            }

            
            user.friends.Add(friend);
            await _context.SaveChangesAsync();

            return _mapper.Map<UserData, UserResource>(friend);
        }

        [HttpPost("parkcheckin/{id}")]
        public async Task<ActionResult<ParkItemResource>> checkintopark (int id)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var securityTokenHandler = new JwtSecurityTokenHandler();
            var decriptedToken = securityTokenHandler.ReadJwtToken(token);
            var claims = decriptedToken.Claims;
            var sub = claims.First(c => c.Type == "sub").Value;
            var user = await _context.UserData.Where(x => x.TokenSub.tokenSub == sub).FirstOrDefaultAsync();
            var park = await _context.ParkItems.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (user == null)
            {
                return StatusCode(403, $"No user" + sub);
            }
            if (park == null)
            {
                return StatusCode(403, $"No friend");
            }

            
            user.Park = park;
            await _context.SaveChangesAsync();
            var result = _mapper.Map<ParkItem, ParkItemResource>(park);
            return result;
        }

        [HttpDelete("removeFriend/{id}")]
        public async Task<ActionResult<UserResource>> userRemoveFriends(int id)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var securityTokenHandler = new JwtSecurityTokenHandler();
            var decriptedToken = securityTokenHandler.ReadJwtToken(token);
            var claims = decriptedToken.Claims;
            var sub = claims.First(c => c.Type == "sub").Value;
            var user = await _context.UserData.Where(x => x.TokenSub.tokenSub == sub).FirstOrDefaultAsync();
            var friend = await _context.UserData.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (user == null)
            {
                return StatusCode(403, $"No user");
            }
            if (friend == null)
            {
                return StatusCode(403, $"No friend");
            }

            
            user.friends.Remove(friend);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostUserById", new { id = user.Id }, _mapper.Map<UserData, UserResource>(user));
        }

        [HttpDelete("leavepark")]
        public async Task<ActionResult<UserResource>> userLeavePark(int id)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var securityTokenHandler = new JwtSecurityTokenHandler();
            var decriptedToken = securityTokenHandler.ReadJwtToken(token);
            var claims = decriptedToken.Claims;
            var sub = claims.First(c => c.Type == "sub").Value;
            var user = await _context.UserData.Where(x => x.TokenSub.tokenSub == sub).FirstOrDefaultAsync();
            if (user == null)
            {
                return StatusCode(403, $"No user");
            }
            

            
            user.Park = null;
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostUserById", new { id = user.Id }, _mapper.Map<UserData, UserResource>(user));
        }

        [HttpPost("{id}")]
        [Authorize(Policy = "IsAdmin")]
        public async Task<ActionResult<UserResource>> PostUserById(string sub)
        {
            var user = await _context.UserData.FindAsync(sub);
            if (user != null)
            {
                return StatusCode(403, "User with auth0 sub, already exists.");
            }
            user = new UserData
            {
                TokenSub = new TokenSub{
                tokenSub =  sub
                },
            };

            _context.UserData.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostUserById", new { id = user.Id }, _mapper.Map<UserData, UserResource>(user));
        }

     
    }
}