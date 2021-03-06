﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
//using TrackMEDXLS.Data;
using TrackMEDXLS.Models;
using TrackMEDXLS.Services;
using Serilog;
using Serilog.Sinks.Email;
using System.Net;
using Serilog.Events;

using WilderBlog.Logger;
using WilderBlog.Services;

namespace TrackMEDXLS
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
            /*
            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }
            */

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
            
            EmailConnectionInfo info = new EmailConnectionInfo()
            {
                EmailSubject = "Test Serilog",
                FromEmail = "jul_soriano@hotmail.com",
                MailServer = "smtp.live.com", // "smtp.live.com"
                NetworkCredentials = new NetworkCredential("jul_soriano@hotmail.com", "acts15:23hot"),
                Port = 587,
                ToEmail = "jul_soriano@yahoo.com"
            };

            // Or This: See https://github.com/serilog/serilog/wiki/Getting-Started
            var uriMongoDB = Configuration.GetValue<string>("MongoSettings:mongoconnection");
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Warning()
                //.WriteTo.LiterateConsole()
                //.WriteTo.RollingFile("logs\\trackmed-{Date}.txt")
                //.WriteTo.Email(info, restrictedToMinimumLevel: LogEventLevel.Warning)   // https://github.com/serilog/serilog/wiki/Configuration-Basics#overriding-per-sink
                //.WriteTo.Seq("http://localhost:5341/")
                .WriteTo.MongoDBCapped(uriMongoDB, collectionName: "logsTrackMED")  // https://github.com/serilog/serilog-sinks-mongodb
                .CreateLogger();

            Log.Information("TrackMEDXLS is launched");
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            // Add our Config object so it can be injected; needs "Microsoft.Extensions.Options.ConfigurationExtensions": "1.0.0"
            // See http://stackoverflow.com/questions/36893326/read-a-value-from-appsettings-json-in-1-0-0-rc1-final
            services.Configure<Settings>(Configuration.GetSection("MongoSettings"));

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            //services.AddTransient<IMailService, MailService>();

            // Added: Add model services
            services.AddSingleton<IEntityService<ActivityType>, EntityService<ActivityType>>();
            services.AddSingleton<IEntityService<Component>, EntityService<Component>>();
            services.AddSingleton<IEntityService<Deployment>, EntityService<Deployment>>();
            services.AddSingleton<IEntityService<Description>, EntityService<Description>>();
            services.AddSingleton<IEntityService<EquipmentActivity>, EntityService<EquipmentActivity>>();
            services.AddSingleton<IEntityService<Location>, EntityService<Location>>();
            services.AddSingleton<IEntityService<Model_Manufacturer>, EntityService<Model_Manufacturer>>();
            services.AddSingleton<IEntityService<Owner>, EntityService<Owner>>();
            services.AddSingleton<IEntityService<ProviderOfService>, EntityService<ProviderOfService>>();
            services.AddSingleton<IEntityService<Status>, EntityService<Status>>();
            services.AddSingleton<IEntityService<SystemsDescription>, EntityService<SystemsDescription>>();
            services.AddSingleton<IEntityService<SystemTab>, EntityService<SystemTab>>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            //loggerFactory.AddEmail(mailService, LogLevel.Critical);

            // See https://www.towfeek.se/2016/06/structured-logging-with-aspnet-core-using-serilog-and-seq/
            loggerFactory.AddSerilog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=SystemTab}/{action=Index}/{id?}");  // template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
