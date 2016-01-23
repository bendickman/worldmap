using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Framework.DependencyInjection;
using TheWorld.Services;
using Microsoft.AspNet.Hosting;
using Microsoft.Framework.Configuration;
using Microsoft.Dnx.Runtime;
using TheWorld.Models;
using Microsoft.Framework.Logging;
using Newtonsoft.Json.Serialization;
using AutoMapper;
using TheWorld.ViewModels;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Authentication.Cookies;
using System.Net;

namespace TheWorld
{
    public class Startup
    {
        public static IConfigurationRoot Configuration;

        public Startup(IApplicationEnvironment appEnv)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(appEnv.ApplicationBasePath)
                .AddJsonFile("config.json")
                .AddEnvironmentVariables();
                

            Configuration = builder.Build();
        }

        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(config =>
                {
#if !DEBUG
                    config.Filters.Add(new RequireHttpsAttribute());//redirect to https for entire site
#endif
                })
                .AddJsonOptions(op =>
                {
                    op.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                }
                );

            services.AddIdentity<WorldUser, IdentityRole>(config =>
            {
                config.User.RequireUniqueEmail = true;
                config.Password.RequiredLength = 8;

                //path to redirect to when accessing a Authorized action
                config.Cookies.ApplicationCookie.LoginPath = "/Auth/Login";
                config.Cookies.ApplicationCookie.Events = new CookieAuthenticationEvents()
                {
                    //redirect is called when unauth is made, this will stop the html being returned in a unauth call
                    OnRedirect = ctx =>
                    {
                        //if an api call is being made and the previous call is ok
                        if (ctx.Request.Path.StartsWithSegments("/api") &&
                            ctx.Response.StatusCode == (int)HttpStatusCode.OK)
                        {
                            ctx.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        }
                        else
                        {
                            ctx.Response.Redirect(ctx.RedirectUri);
                        }

                        

                        return Task.FromResult(0);
                    }
                };
            })
            .AddEntityFrameworkStores<WorldContext>();

            services.AddLogging();

            services.AddEntityFramework()
                .AddSqlServer()
                .AddDbContext<WorldContext>();

            
            services.AddTransient<TheWorldContextSeedData>();
            services.AddScoped<IWorldRepository, WorldRepository>();

            services.AddScoped<IMailService, DebugMailService>();

            services.AddScoped<CoordService>();

        }

        public async void Configure(IApplicationBuilder app, TheWorldContextSeedData seedContext, ILoggerFactory logger)
        {
            logger.AddDebug(LogLevel.Debug);

            app.UseStaticFiles();// allow self hosting to use static files i.e. files in the wwwroot

            //order is important
            app.UseIdentity();

            //config automapper to use Trip > TripViewModel and the reverse
            //also supports mapping of collections i.e. List<Trip> to List<TripViewModel>
            Mapper.Initialize(config =>
            {
                config.CreateMap<Trip, TripViewModel>().ReverseMap();
                config.CreateMap<Stop, StopViewModel>().ReverseMap();
            });

            //listen for mvc requests
            app.UseMvc(config =>
            {
                config.MapRoute(
                    name: "Default",
                    template: "{controller}/{action}/{id?}",//optional id
                    defaults: new { controller = "App", action = "Index" }
                    );
            });

            await seedContext.EnsureSeedDataAsync();
        }
    }
}
