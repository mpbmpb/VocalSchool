using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using VocalSchool.Controllers;
using VocalSchool.Data;
using VocalSchool.Models;
using VocalSchool.ViewModels;
using Xunit;

namespace VocalSchool.Test.Infrastructure
{
    public class UnitTestBase : IClassFixture<ConfigFixture>,IDisposable
    {
        protected readonly SchoolContext Seedcontext;
        protected readonly SchoolContext Context;
        protected readonly SchoolContext Resultcontext;
        private ConfigFixture _fixture;
        
        public UnitTestBase(ConfigFixture fixture)
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

            DbInitializer.InitializeDb(Seedcontext);
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

        protected static async Task<T> GetModel<T>(Func<int?, Task<IActionResult>> method, int id)
        {
            var actionResult = await method(id);
            var model = actionResult.As<ViewResult>().Model.As<T>();
            return model;
        }
        
        protected async Task MakeNewCourse(int courseDesignId)
        {
            var courseView = new CourseViewModel(new List<CourseDesign>(), "http://www.completevocaltraining.nl")
            {
                Course = new Course
                {
                    Name = "test",
                    CourseDesign = new CourseDesign {CourseDesignId = courseDesignId}
                }
            };
            var courseContr = new CourseController(Seedcontext);
            await courseContr.Create(courseView);
        }

        
    }
}
