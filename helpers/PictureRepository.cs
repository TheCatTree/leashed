using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using Google.Cloud.Storage.V1;
using System.Net.Http;
using System.Collections.Generic;
using Google.Apis.Auth.OAuth2;
using leashed.Controllers.Resources;

namespace leashed.helpers
{
    public class PictureRepository : IPictureRepository
    {
        private string s3NinjaAccessKey = "AKIAIOSFODNN7EXAMPLE";
        private string s3NinjaSecretKey = "wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY";
        private string s3BucketName = "s3test";

        private string gcsBucketName = "dev-growler-images";

        private string gcsProjectId = "leashed-1585625476385";
        private AmazonS3Config s3Config = new AmazonS3Config();

        private  Google.Apis.Storage.v1.Data.Bucket gcsBucket;
        
        private static IAmazonS3 s3Client;

        private static StorageClient gcsClient;
        public PictureRepository(){
            s3Config.ServiceURL= "http://192.168.99.100:9444/s3/";
            s3Client = new AmazonS3Client(
                s3NinjaAccessKey,
                s3NinjaSecretKey,
                s3Config
            );

            
             try{
                gcsClient = StorageClient.Create();
                

            } catch (InvalidOperationException e ) {
                Console.WriteLine("In Bucket retrieval");
                Console.WriteLine("catched context error: " + e);
            } catch (Exception e) {
                Console.WriteLine("In Bucket retrieval");
                Console.WriteLine("Caught random exception: " + e);
            }

            try{
                
                gcsBucket = gcsClient.GetBucket(gcsBucketName);

            } catch (InvalidOperationException e ) {
                Console.WriteLine("In Bucket retrieval");
                Console.WriteLine("catched context error: " + e);
            } catch (Exception e) {
                Console.WriteLine("In Bucket retrieval");
                Console.WriteLine("Caught random exception: " + e);
            }
            
           //gcsBucket = gcsClient.CreateBucket(gcsProjectId,gcsBucketName);
        }

       

        public bool canAccessImage(string id)
        {

            return false;
        }

        public async Task<PutBucketResponse> setupBucket()
        {   
             var putBucketRequest = new PutBucketRequest
                    {
                        BucketName = s3BucketName,
                        UseClientRegion = true
                    };
            PutBucketResponse putBucketResponse = await s3Client.PutBucketAsync(putBucketRequest);
            return putBucketResponse;

        }

        public Task<secureURLResource> uploadImageURL(string key, double duration)
        {   
            return makeImageURL(key, duration, HttpMethod.Put);
        }

        public Task<secureURLResource> getImageURL(string key, double duration)
        {   
            return makeImageURL(key, duration, HttpMethod.Get);
        }

        private static string GeneratePreSignedURL(string objectKey, string bucketName, double duration)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Key        = objectKey,
                Verb       = HttpVerb.PUT,
                Expires    = DateTime.UtcNow.AddHours(duration)
            };

           string url = s3Client.GetPreSignedURL(request);
           return url;
        }

        public Task<secureURLResource> makeImageURL(string key, double duration, HttpMethod method)
        {   
            Dictionary<string, IEnumerable<string>> contentHeaders;
            if(method == HttpMethod.Get){

                contentHeaders = new Dictionary<string, IEnumerable<string>> 
                    {
                        
                    };
            }else {
                contentHeaders = new Dictionary<string, IEnumerable<string>> 
                    {
                        { "Content-Type", new[] { "image/*" } }
                    };
            }

            UrlSigner.RequestTemplate requestTemplate = UrlSigner.RequestTemplate
            .FromBucket(gcsBucketName)
            .WithObjectName(key)
            .WithHttpMethod(method)
            .WithContentHeaders(contentHeaders);

            ServiceAccountCredential credential;
            using (FileStream fs = File.OpenRead(Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS")))
            {
                credential = ServiceAccountCredential.FromServiceAccountData(fs);
            }
            UrlSigner.Options options = UrlSigner.Options.FromDuration(TimeSpan.FromHours(duration)).WithSigningVersion(SigningVersion.V4);
            
            UrlSigner urlSigner = UrlSigner.FromServiceAccountCredential(credential);
            string url = urlSigner.Sign(requestTemplate, options);
            //var url = GeneratePreSignedURL(key, s3BucketName, 1);
            secureURLResource surl = new secureURLResource();
            surl.URL = url;
            return Task.FromResult(surl);
        }
    }
}