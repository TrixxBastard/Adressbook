﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Adressbook.Data;
using Adressbook.Models;
using Adressbook.Services;
using Adressbook.Interfaces;
using System.Globalization;
using Adressbook.Resources;
using Microsoft.Extensions.Localization;
using Adressbook.Controllers;
using Adressbook.Localizers;

namespace Adressbook
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddTransient<IStringLocalizer<HomeController>,
                HomeStringLocalizer>();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();


            ITimeProvider myFakeTimeProvider = new FakeTimeProvider();
            myFakeTimeProvider.Now = new DateTime(2018, 2, 1);
            services.AddSingleton<ITimeProvider>(new FakeTimeProvider());

            services.AddTransient<IStringLocalizer<HomeController>,
                DbLocalizer>();

            services.AddLocalization();
            services.AddMvc()
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                    factory.Create(typeof(SharedResources));
                });

        } 
        

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ApplicationDbContext dbContext, ApplicationDbContext context)
        {
           
                
      

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.Use((request, next) =>
            {
                var sv = new CultureInfo("sv-SE");
                System.Threading.Thread.CurrentThread.CurrentCulture = sv;
                System.Threading.Thread.CurrentThread.CurrentUICulture = sv;
                return next();
            });

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=People}/{action=Index}/{id?}/{slug?}");
            });

           // DbSeed.Seed(context);
            DbSeed.Seed(dbContext);
        }

    }
}
