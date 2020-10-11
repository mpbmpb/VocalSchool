using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using VocalSchool.Models;

namespace VocalSchool.Test.Infrastructure
{
    public class VocalSchoolTestBase : IDisposable
    {
        protected readonly SchoolContext Seedcontext;
        protected readonly SchoolContext Context;
        protected readonly SchoolContext Resultcontext;
        private bool _isLazyLoading = true;
        
        public VocalSchoolTestBase()
        {
            // CheckLazyLoadingAppsetting();
            var options = new DbContextOptionsBuilder<SchoolContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            if (_isLazyLoading)
            {
                options = new DbContextOptionsBuilder<SchoolContext>()
                .UseLazyLoadingProxies()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            }
          
            Seedcontext = new SchoolContext(options);
            Context = new SchoolContext(options);
            Resultcontext = new SchoolContext(options);

            Context.Database.EnsureCreated();

            VocalSchoolInitializer.Initialize(Seedcontext);
        }

        public void Dispose()
        {
            Seedcontext.Database.EnsureDeleted();
            Context.Database.EnsureDeleted();
            Resultcontext.Database.EnsureDeleted();

            Seedcontext.Dispose();
            Context.Dispose();
            Resultcontext.Dispose();
        }

        public bool ViewContainsDay(string controllerName, string viewName)
        {
            bool day = false;
            string[] viewString = File.ReadAllLines($"../../../../VocalSchool/Views/{controllerName}/{viewName}.cshtml");
            for (int line = 0; line < viewString.Length; line++)
            {
                if (viewString[line].Contains(".Day"))
                {
                    day = true;
                    break;
                }
            }
            return day;
        }

        private void CheckLazyLoadingAppsetting()
        {
            var dir = Directory.GetCurrentDirectory();
            var path = Path.GetDirectoryName(dir
                .Substring(0, dir.IndexOf("VocalSchool.Test")));
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"{path}/VocalSchool/appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            
            IConfigurationRoot configuration = builder.Build();
            _isLazyLoading = configuration.GetValue<bool>("LazyLoading");
        }
    }
}
