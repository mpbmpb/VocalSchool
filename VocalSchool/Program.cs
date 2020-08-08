using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VocalSchool.Models;

namespace VocalSchool
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}



//TODO disable enter key as submit button

//TODO change redirect on save to back to last page

//TODO refactor _context out of controllers

//TODO make calendar date picker

//TODO make calendar controller that displays all events

//TODO make student class

//TODO make teacher class

//TODO make enrollment class (m2m)

//TODO add login / roles