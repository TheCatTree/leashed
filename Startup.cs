using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using leashApi.Models;
using Npgsql;
using leashed.helpers;
using Microsoft.IdentityModel.Logging;
using leashed.Authorization;

namespace leashApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        // touched
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            IdentityModelEventSource.ShowPII = true;
            // var connectionString = Configuration["PostgreSql:ConnectionString"];
            // var dbPassword = Configuration["PostgreSql:DbPassword"];
            // var builder = new NpgsqlConnectionStringBuilder(connectionString){
            //     Password = dbPassword
            // };

                services.AddCors(options =>  
                {  
                    options.AddPolicy("TestApp",  
                    builder => builder.WithOrigins("https://localhost:7001").AllowAnyHeader());  
                });

                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(options =>
                {
                    options.Authority = $"{Configuration["Auth0:Domain"]}";
                    options.Audience = Configuration["Auth0:Audience"];
                });

                services.AddAuthorization(options =>
                {
                    options.AddPolicy("IsAdmin", policy => policy.Requirements.Add(new ILeashedAuthorizationHandlerRequirement()));
                });
                services.AddSingleton<IAuthorizationHandler, ILeashedAuthorizationHandler>();
            string connectionString = null;
            Console.WriteLine("About to make the string for db");
            //Console.WriteLine(Helpers.connectionStringMaker());
            string envVar = Environment.GetEnvironmentVariable("DATABASE_URL");
            if(!string.IsNullOrEmpty(envVar)){
                Console.WriteLine("Getting connection String from DATABASE_URL");
                
                //parse database URL. Format is postgres://<username>:<password>@<host>/<dbname>
                var uri = new Uri(envVar);
                var username = uri.UserInfo.Split(':')[0];
                var password = uri.UserInfo.Split(':')[1];
                connectionString = 
                "Host = " + uri.AbsolutePath.Substring(0) +
                "; Database=" + uri.AbsolutePath.Substring(1) +
                "; Username=" + username +
                "; Password=" + password + 
                "; Port=" + uri.Port +
                "; SSL Mode=Require; Trust Server Certificate=true;";
                Console.WriteLine(" DATABASE_URL = " + envVar);
                Console.WriteLine(" connectionString = " + connectionString);

                try{
                    
                    services.AddDbContext<ParkContext>(opt =>
                    opt.UseNpgsql(connectionString));
                    services.AddControllers();
                    services.AddScoped<IDbInitializer, DbInitializer> ();

                } catch(InvalidOperationException e){
                    Console.WriteLine(" -- Caught exception. opening connection to local db " + e);
                    Console.WriteLine(" -- --");
                }

            }
            else{
                try{ //trying to create and connecft with environemnt variables. This will work build only.
                    Console.WriteLine(" Attempting connection to db with environment vars ");

                    connectionString = Helpers.connectionStringMaker();
                    Console.WriteLine(" -- we are now past thrown error ");
                    services.AddDbContext<ParkContext>(opt =>
                    opt.UseNpgsql(connectionString));
                    services.AddControllers();
                    services.AddScoped<IDbInitializer, DbInitializer> ();

                } catch(InvalidOperationException e){

                    Console.WriteLine(" -- Caught exception. opening connection to local db " + e);
                    Console.WriteLine(" -- --");
                    Console.WriteLine(" -- now beginning to build string from config files");
                    connectionString = Configuration["PostgreSql:ConnectionString"];
                    var dbPassword = Configuration["PostgreSql:DbPassword"];
                    Console.WriteLine("connection string: " + connectionString);
                    Console.WriteLine("db password: " + dbPassword);
                    var builder = new NpgsqlConnectionStringBuilder(connectionString){
                        Password = dbPassword
                    };
                    services.AddDbContext<ParkContext>(opt =>
                    opt.UseNpgsql(builder.ConnectionString));
                    services.AddControllers();
                    services.AddScoped<IDbInitializer, DbInitializer> ();
                    services.AddScoped<IPictureRepository, PictureRepository> ();
                
                } 
            }

            //services.AddControllers();
        
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            
            Console.WriteLine("--getting context");
            //added
            try{
                var context = app.ApplicationServices.GetService<ParkContext>();

                Console.WriteLine("Context type is: {0}",context.GetType());
                
                if(!context.Database.EnsureCreated()){
                   Console.WriteLine("--about to run migration");
                //EnsureCreated makes the database and returns true if it dose can not then use migrations
                context.Database.Migrate();
                }

            } catch (InvalidOperationException e ) {
                Console.WriteLine("In database creation block.");
                Console.WriteLine("catched context error: " + e);
            } catch (Exception e) {
                Console.WriteLine("In database creation block.");
                Console.WriteLine("Caught random exception: " + e);
            }

            var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory> ();
            using (var scope = scopeFactory.CreateScope ()) {
                    var dbInitializer = scope.ServiceProvider.GetService<IDbInitializer> ();
                //dbInitializer.Initialize ();
                    dbInitializer.SeedData ();
                }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("TestApp");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
