using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using SalesWebMvc.Data;
using SalesWebMvc.Services;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace SalesWebMvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<SalesWebMvcContext>(options =>
                options.UseMySql
                (
                    builder.Configuration.GetConnectionString("SalesWebMvcContext"),
                    new MySqlServerVersion(new Version(8, 0, 32)),
                    builder => builder.MigrationsAssembly("SalesWebMvc")
                ));

            CultureInfo enUs = new("en-US");
            RequestLocalizationOptions requestLocalizationOptions = new()
            {
                DefaultRequestCulture = new RequestCulture(enUs),
                SupportedCultures = new List<CultureInfo> { enUs },
                SupportedUICultures = new List<CultureInfo> { enUs },
            };
                
            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<SellerService>();
            builder.Services.AddScoped<DepartamentService>();
            builder.Services.AddScoped<SeedingService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if(!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            /*else // this seed the database
            {
                ServiceProvider serviceProvider = builder.Services.BuildServiceProvider()!;
                SalesWebMvcContext serviceContext= serviceProvider.GetRequiredService<SalesWebMvcContext>();
                SeedingService s = new(serviceContext.GetService<SalesWebMvcContext>());
                s.Seed();
            }*/

            app.UseRequestLocalization(requestLocalizationOptions);

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}