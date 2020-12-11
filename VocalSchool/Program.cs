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

//TODO add method to remove uid from name & display in readonly field to all edit methods
//TODO add method to reform name from readonly uid + name to all edit methods

//TODO add uid check to all controller edit methods

//TODO add casccade delete method to coursecontroller delete

//TODO add CreateCourseDesignFromCourse method to CourseController