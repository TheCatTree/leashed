using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using leashed.Controllers.Resources;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace leashed.helpers
{
    public interface IPictureRepository
    {
        public Task<secureURLResource> getImageURL(string key, double duration);


         bool canAccessImage(string id);

        Task<PutBucketResponse> setupBucket();

        Task<secureURLResource> uploadImageURL(string key, double duration);


    }
}