using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using VocalSchool.Models;
using Xunit;

namespace VocalSchool.Test.Infrastructure
{
    public class VocalSchoolTestBase : IClassFixture<ConfigFixture>,IDisposable
    {
        protected readonly SchoolContext Seedcontext;
        protected readonly SchoolContext Context;
        protected readonly SchoolContext Resultcontext;
        private ConfigFixture _fixture;
        
        public VocalSchoolTestBase(ConfigFixture fixture)
        {
            _fixture = fixture;
            
            var options = new DbContextOptionsBuilder<SchoolContext>()
                .UseLazyLoadingProxies(fixture.IsLazyLoading)
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
                
            Seedcontext = new SchoolContext(options);
            Context = new SchoolContext(options);
            Resultcontext = new SchoolContext(options);

            Seedcontext.Database.EnsureCreated();

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
        
        protected async Task<T> GetModel<T>(Func<int?, Task<IActionResult>> method, int id)
        {
            var actionResult = await method(id);
            var model = actionResult.As<ViewResult>().Model.As<T>();
            return model;
        }

        
    }
}
