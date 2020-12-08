using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3.Model;
using leashApi.Models;
using leashed.Controllers.Resources;
using leashed.helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace leashApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("TestApp")] 
    public class PicturesController : Controller
    {

        private readonly ParkContext _context;
        private readonly IPictureRepository _pictureRepository;

        public PicturesController(ParkContext context, IPictureRepository pictureRepository)
        {
            _context = context;
            _pictureRepository = pictureRepository;
        }

        [HttpGet("upload/{name}")]
        public async Task<ActionResult<secureURLResource>> GetUploadURL(string name)
        {
           var GUID = Guid.NewGuid();
           var key = "userimages/" + GUID.ToString();
            var picture = new Picture();
            picture.FileName = name;
            picture.Key = key;
            var token = await HttpContext.GetTokenAsync("access_token");
            var securityTokenHandler = new JwtSecurityTokenHandler();
            var decriptedToken = securityTokenHandler.ReadJwtToken(token);
            var claims = decriptedToken.Claims;
            var sub = claims.First(c => c.Type == "sub").Value;
            var user = await _context.UserData.Where(x => x.TokenSub == sub).FirstOrDefaultAsync();
            picture.UserDataId = user.Id;
            _context.Pictures.Add(picture);
            await _context.SaveChangesAsync();
            secureURLResource signedURL =  _pictureRepository.uploadImageURL(key, 1);
            signedURL.Id = picture.Id;
            return  signedURL;
        }

        [HttpGet("image/{id}")]
        public async Task<ActionResult<secureURLResource>> GetImageURL(int id)
        {
            var picture = await _context.Pictures.Where(x => x.Id == id).FirstOrDefaultAsync();
            if(!picture.IsPublic)
            {
                var token = await HttpContext.GetTokenAsync("access_token");
                var securityTokenHandler = new JwtSecurityTokenHandler();
                var decriptedToken = securityTokenHandler.ReadJwtToken(token);
                var claims = decriptedToken.Claims;
                var sub = claims.First(c => c.Type == "sub").Value;
                var user = await _context.UserData.Where(x => x.TokenSub == sub).FirstOrDefaultAsync();
                

                if(!Helpers.canAccess(picture,user,Helpers.AccessLevel.read)){
                    return StatusCode(403, $"Access Denied"); 
                }
            }
            Console.WriteLine("---------------------get picture-----------------------");

            secureURLResource pictureResource = _pictureRepository.uploadImageURL(picture.Key, 1);
            pictureResource.Id = picture.Id;

            return pictureResource;
        }

        [HttpGet("images")]
        public async Task<ActionResult<IEnumerable<secureURLResource>>> GetImagesURL()
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var securityTokenHandler = new JwtSecurityTokenHandler();
            var decriptedToken = securityTokenHandler.ReadJwtToken(token);
            var claims = decriptedToken.Claims;
            var sub = claims.First(c => c.Type == "sub").Value;
            var user = await _context.UserData.Where(x => x.TokenSub == sub).FirstOrDefaultAsync();
            
            var Surls = new Collection<secureURLResource>();;
            Console.WriteLine("---------------------get pictures-----------------------");
            var pictures = await _context.Pictures.Where(x => x.UserDataId == user.Id).ToListAsync();
            foreach(Picture picture in pictures){
                if(!picture.IsPublic)
                {   
                    if(!Helpers.canAccess(picture,user,Helpers.AccessLevel.read)){
                        continue; 
                    }
                }
                var p = picture.Key;
                var x = _pictureRepository.getImageURL(p, 1);
                x.Id = picture.Id;
                Surls.Add(x);
            }
            return Surls;
        }

        [HttpGet("makebucket")]
        public async Task<ActionResult<PutBucketResponse>> MakeBucket()
        {
           
           
            return await _pictureRepository.setupBucket();
        }


     
    }
}