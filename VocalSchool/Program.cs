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
//TODO check delete methods for concurrencyException try/catch

//TODO change redirect on save to back to last page

//TODO add delete button to edit views

//TODO make method for tables in views

//TODO make calendar controller that displays all events

//TODO make student class

//TODO make teacher class

//TODO make enrollment class (m2m)

//TODO add login / roles

//TODO add CreateCourseDesignFromCourse method to CourseController