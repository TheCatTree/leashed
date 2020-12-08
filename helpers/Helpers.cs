using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Collections;
using leashApi.Models;
using System.Collections.Generic;
using leashed.Controllers.Resources;
using System.Net.Http;
using Google.Cloud.Storage.V1;
using Google.Apis.Auth.OAuth2;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization.Formatters.Binary;

public class Helpers{
    
    public enum AccessLevel{
            read,
            edit
        }

        public static bool canAccess(Picture picture, UserData user, AccessLevel accessLevel){
            if(accessLevel == AccessLevel.read){
                if(picture.IsPublic) return true;
                if(picture.UserDataId == user.Id) return true;
                if(picture.canRead.Contains(user.TokenSub))return true;
            }
            if(accessLevel == AccessLevel.edit){
                if(picture.UserDataId == user.Id) return true;
                if(picture.canEdit.Contains(user.TokenSub))return true;
            }
            return false;
        }
        public static string connectionStringMaker()
        {
            Console.WriteLine("--in connection string helper method" );
            Console.WriteLine("--Environment variables:");
            foreach (DictionaryEntry de in Environment.GetEnvironmentVariables())
                { 
                Console.WriteLine("  {0} = {1}", de.Key, de.Value);
                }
            
            if(Environment.GetEnvironmentVariable("DATABASE_URL") != null){
                 Console.WriteLine("using generated string from env vars" );
                string connectionURL = Environment.GetEnvironmentVariable("DATABASE_URL");

                connectionURL.Replace("//", "");

                char[] delimeterChars = {'/',':','@','?'};

                string[] strConn = connectionURL.Split(delimeterChars);
                strConn = strConn.Where(x => !string.IsNullOrEmpty(x)).ToArray();

                string User = strConn[1];
                string Pass = strConn[2];
                string Server = strConn[3];
                string Database = strConn[5];
                string Port = strConn[4];

                string connectionString = $"host={Server};port={Port};database={Database};uid={User};pwd={Pass};sslmode=Require;Trust Server Certificate=true;Timeout=1000";
                Console.WriteLine(connectionString + ": heroku db connection string");
                return connectionString;
            } else{
               
                throw new System.InvalidOperationException("--Environment variables are not declared correctly");
 
            }
        }
         public static secureURLResource makeImageURL(string key, double duration, HttpMethod method)
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
            .FromBucket("dev-growler-images")
            .WithObjectName(key)
            .WithHttpMethod(method)
            .WithContentHeaders(contentHeaders);
            ServiceAccountCredential credential;
            if(File.Exists(Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS"))){
                using (FileStream fs = File.OpenRead(Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS")))
                    {
                        credential = ServiceAccountCredential.FromServiceAccountData(fs);
                    }
            }
            else{
                using (FileStream fs = File.OpenRead("../" + Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS")))
                    {
                        credential = ServiceAccountCredential.FromServiceAccountData(fs);
                    }    
            }
            
            UrlSigner.Options options = UrlSigner.Options.FromDuration(TimeSpan.FromHours(duration)).WithSigningVersion(SigningVersion.V4);
            
            UrlSigner urlSigner = UrlSigner.FromServiceAccountCredential(credential);
            string url = urlSigner.Sign(requestTemplate, options);
            //var url = GeneratePreSignedURL(key, s3BucketName, 1);
            secureURLResource surl = new secureURLResource();
            surl.URL = url;
            return surl;
        }
 }
    