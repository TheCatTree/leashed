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
            
           // await _pictureRepository.setupBucket();
           // 
           // var signedURL = await _pictureRepository.uploadImageURL(GUID, 12);
           // var picture = new Picture();
           // picture.GivenName = name;
           // picture.FileName = GUID;
           // picture.URL = signedURL;
           // 
           // 
           // Console.WriteLine("---------------------Picture Created-----------------------");
           // Console.WriteLine(signedURL);
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
            secureURLResource signedURL = await _pictureRepository.uploadImageURL(key, 1);
            signedURL.Id = picture.Id;
            return  signedURL;
        }

        [HttpGet("image/{id}")]
        public async Task<ActionResult<secureURLResource>> GetImageURL(int id)
        {
            var picture = await _context.Pictures.Where(x => x.Id == id).FirstOrDefaultAsync();
            Console.WriteLine("---------------------get picture-----------------------");

            secureURLResource pictureResource = await _pictureRepository.uploadImageURL(picture.Key, 1);
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
                var p = picture.Key;
                var x = await _pictureRepository.getImageURL(p, 1);
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