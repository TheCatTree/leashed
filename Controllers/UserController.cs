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
using leashed.Authorization.ExtensionMethods;
using leashed.Services;

namespace leashApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("TestApp")] 
    public class UserController : Controller
    {
        private readonly ParkContext _context;
        private readonly IMapper _mapper;
        private readonly ITokenUserResolverService _user;

        public UserController(ParkContext context, IMapper mapper, ITokenUserResolverService user)
        {
            _context = context;
            _mapper = mapper;
            _user = user;
        }

        [HttpGet]
        public async Task<IActionResult> GetUser(){

            var user = await _context.UserData.Where(x => x.TokenSub.tokenSub == _user.getSub()).FirstOrDefaultAsync();
            Console.WriteLine("---------------------sub-----------------------");
            Console.WriteLine(_user.getSub());
           if (user == null)
           {
                return StatusCode(403, $"No User Data: {_user.getSub()} ");
            }

            _context.Entry(user).Reference(u => u.TokenSub).Load();
            _context.Entry(user).Collection(u => u.Friends).Load();


            return Ok(_mapper.Map<UserData,UserResource>(user));
        }


        [HttpGet("listusers")]
        [Authorize(Policy = "IsAdmin")]
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

        [HttpGet("listusers/public")]
        public async Task<ActionResult<ICollection<UserResourcePublic>>> ListUserPublic(){
            
            var users = await _context.UserData.ToListAsync();
            var usersOut = new Collection<UserResourcePublic>();
            foreach(UserData user in users){
                if(user.PrivacyLevel == PrivacyLevel.Hidden){
                    continue;
                }
                var x = new UserResourcePublic();
                x.Id = user.Id;
                x.Name = user.Name;
                usersOut.Add(x);
            }
            return usersOut;
        }

        [HttpGet("friends")]
        public async Task<ActionResult<ICollection<UserResource>>> GetFriends(){

            var user = await _context.UserData.Where(x => x.TokenSub.tokenSub == _user.getSub()).FirstOrDefaultAsync();
            Console.WriteLine("---------------------sub-----------------------");
            Console.WriteLine(_user.getSub());
           if (user == null)
           {
                return StatusCode(403, $"No User Data: {_user.getSub()} ");
            }
            _context.Entry(user).Collection(u => u.Friends).Load();
            var usersOut = new Collection<UserResource>();
            var friends = user.Friends.ToList();
            Console.WriteLine("---------------------users friend count {0} -----------------------", friends.Count());
            foreach(UserData friend in friends){
                var x = new UserResource();
                x = _mapper.Map<UserData,UserResource>(friend);
                usersOut.Add(x);
            }

            return usersOut;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id){

            
            var User = await _context.UserData.FindAsync(id);
            if (User == null)
            {
                return NotFound();
            }
            _context.Entry(User).Reference(u => u.TokenSub).Load();
            _context.Entry(User).Collection(u => u.Friends).Load();


            if(_user.isAdmin()){
                return Ok(_mapper.Map<UserData,UserResource>(User));
            }

            var Requester = await _context.UserData.Where(x => x.TokenSub.tokenSub == _user.getSub()).FirstOrDefaultAsync();
            if (Requester == null)
            {
                return NotFound();
            }

            if(User.PrivacyLevel == PrivacyLevel.Public || User.Friends.Contains(Requester)){
                return Ok(_mapper.Map<UserData,UserResource>(User));
            }
            
            return NotFound();

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> deleteUserById(int id){

            
            var user = await _context.UserData.FindAsync(id);
            if (User == null)
            {
                return NotFound();
            }

            if(_user.isAdmin()|| user.TokenSub.tokenSub == _user.getSub()){
                _context.UserData.Remove(user);
                await _context.SaveChangesAsync();
                return Ok(_mapper.Map<UserData,UserResource>(user));
            }
            
            return NotFound();

        }

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetUserByName(String name){

            
            var User = await _context.UserData.FirstOrDefaultAsync(x => x.Name == name);
            if (User == null)
            {
                return NotFound();
            }

            if(_user.isAdmin()){
                return Ok(_mapper.Map<UserData,UserResource>(User));
            }

            var Requester = await _context.UserData.Where(x => x.TokenSub.tokenSub == _user.getSub()).FirstOrDefaultAsync();
            if (Requester == null)
            {
                return NotFound();
            }

            if(User.PrivacyLevel == PrivacyLevel.Public || User.Friends.Contains(Requester)){
                return Ok(_mapper.Map<UserData,UserResource>(User));
            }
            
            return NotFound();

        }

        [HttpGet("public/{id}")]
        public async Task<IActionResult> GetUserByIdPublic(int id){

            var user = await _context.UserData.FindAsync(id);

            if (user == null || user.PrivacyLevel == PrivacyLevel.Hidden)
            {
                return NotFound();
            }


            return Ok(new UserResourcePublic{
                Id = user.Id,
                Name = user.Name
             });
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<UserResource>> Postuser()
        {
            var user = await _context.UserData.Where(x => x.TokenSub.tokenSub == _user.getSub()).FirstOrDefaultAsync();
            Console.WriteLine("---------------------sub-----------------------");
            Console.WriteLine(_user.getSub());
            Console.WriteLine("---------------------USER FROM POST-----------------------");
            Console.WriteLine(user);
            if (user != null)
            {
                return StatusCode(403, $"User with auth0 sub, already exists: {_user.getSub()} {user}");
            }

            user = new UserData
            {
                TokenSub = new TokenSub{
                tokenSub =  _user.getSub()
                },
                Name = "Not Set Yet"
            };

            _context.UserData.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostUserById", new { id = user.Id }, _mapper.Map<UserData, UserResource>(user));
        }

        [HttpPost("addDummy")]
        [Authorize(Policy= "IsAdmin")]
        public async Task<ActionResult<UserResource>> addDummyUser(UserResource userResource)
        {
            

            var user = new UserData
            {
                TokenSub = new TokenSub{
                tokenSub =  Guid.NewGuid().ToString()
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
            var user = await _context.UserData.Where(x => x.TokenSub.tokenSub == _user.getSub()).FirstOrDefaultAsync();
            var friend = await _context.UserData.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (user == null)
            {
                return StatusCode(403, $"No user");
            }
            if (friend == null)
            {
                return StatusCode(403, $"No friend");
            }

            
            user.Friends.Add(friend);
            await _context.SaveChangesAsync();

            return _mapper.Map<UserData, UserResource>(friend);
        }

        [HttpPost("addFriendToUser/{userId}&{friendId}")]
        public async Task<ActionResult<UserResource>> addFriendToUser(int userId, int friendId)
        {
            var user = await _context.UserData.Where(x => x.Id == userId).FirstOrDefaultAsync();
            var friend = await _context.UserData.Where(x => x.Id == friendId).FirstOrDefaultAsync();
            if (user == null)
            {
                return StatusCode(403, $"No user");
            }
            if(!_user.isAdmin() && _user.getSub() != user.TokenSub.tokenSub)
            {
                return StatusCode(403, $"Not Authorized to modify this user");
            }
            if (friend == null)
            {
                return StatusCode(403, $"No friend");
            }


            
            user.Friends.Add(friend);
            await _context.SaveChangesAsync();

            return _mapper.Map<UserData, UserResource>(friend);
        }

        [HttpPost("parkcheckin/{id}")]
        public async Task<ActionResult<ParkItemResource>> checkintopark (int id)
        {
            var user = await _context.UserData.Where(x => x.TokenSub.tokenSub == _user.getSub()).FirstOrDefaultAsync();
            var park = await _context.ParkItems.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (user == null)
            {
                return StatusCode(403, $"No user" + _user.getSub());
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

        [HttpPost("parkcheckuserin/{userId}&{parkId}")]
        public async Task<ActionResult<ParkItemResource>> checkUserInToPark (int userId, int parkId)
        {
            var user = await _context.UserData.Where(x => x.Id == userId).FirstOrDefaultAsync();
            var park = await _context.ParkItems.Where(x => x.Id == parkId).FirstOrDefaultAsync();
            if (user == null)
            {
                return StatusCode(403, $"No user" + _user.getSub());
            }
            if(!_user.isAdmin() && _user.getSub() != user.TokenSub.tokenSub)
            {
                return StatusCode(403, $"Not Authorized to modify this user");
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
            var user = await _context.UserData.Where(x => x.TokenSub.tokenSub == _user.getSub()).FirstOrDefaultAsync();
            var friend = await _context.UserData.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (user == null)
            {
                return StatusCode(403, $"No user");
            }
            
            if (friend == null)
            {
                return StatusCode(403, $"No friend");
            }

            
            user.Friends.Remove(friend);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostUserById", new { id = user.Id }, _mapper.Map<UserData, UserResource>(user));
        }

        [HttpDelete("removeFriendFromUser/{userId}&{friendId}")]
        public async Task<ActionResult<UserResource>> removeFriendFromUser(int userId, int friendId)
        {
            var user = await _context.UserData.Where(x => x.Id == userId).FirstOrDefaultAsync();
            var friend = await _context.UserData.Where(x => x.Id == friendId).FirstOrDefaultAsync();
            if (user == null)
            {
                return StatusCode(403, $"No user");
            }
            if(!_user.isAdmin() && _user.getSub() != user.TokenSub.tokenSub)
            {
                return StatusCode(403, $"Not Authorized to modify this user");
            }
            if (friend == null)
            {
                return StatusCode(403, $"No friend");
            }

            
            user.Friends.Remove(friend);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostUserById", new { id = user.Id }, _mapper.Map<UserData, UserResource>(user));
        }

        [HttpDelete("leavepark")]
        public async Task<ActionResult<UserResource>> userLeavePark()
        {
            var user = await _context.UserData.Where(x => x.TokenSub.tokenSub == _user.getSub()).FirstOrDefaultAsync();
            if (user == null)
            {
                return StatusCode(403, $"No user");
            }
            

            
            user.Park = null;
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostUserById", new { id = user.Id }, _mapper.Map<UserData, UserResource>(user));
        }

        [HttpDelete("leavepark/{id}")]
        public async Task<ActionResult<UserResource>> userLeavePark(int id)
        {
            var user = await _context.UserData.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (user == null)
            {
                return StatusCode(403, $"No user");
            }
            if(!_user.isAdmin() && _user.getSub() != user.TokenSub.tokenSub)
            {
                return StatusCode(403, $"Not Authorized to modify this user");
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

        [HttpPut("{id}")]
        public async Task<ActionResult<UserResource>> modifyUser(int id, UserResource userValues)
        {
            var user = await _context.UserData.Where(x => x.TokenSub.tokenSub == _user.getSub()).FirstOrDefaultAsync();
            var userToModify = await _context.UserData.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (user == null)
            {
                return StatusCode(403, $"No editor Data");
            }
            if (userToModify == null)
            {
                return StatusCode(403, $"No user to modify");
            }

            if(_user.isAdmin() || user.Id == userToModify.Id){
                userToModify.Name = userValues.Name;
                _context.Entry(userToModify).State = EntityState.Modified;
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
            

                return _mapper.Map<UserData, UserResource>(userToModify);
            }

            return NotFound();
            
        }

     
    }
}