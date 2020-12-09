using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

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

//TODO make method for tables in views

//TODO make calendar date picker

//TODO make calendar controller that displays all events

//TODO make student class

//TODO make teacher class

//TODO make enrollment class (m2m)

//TODO add login / roles

//TODO filter edit views based on uid

//TODO add edit btns for items in edit views