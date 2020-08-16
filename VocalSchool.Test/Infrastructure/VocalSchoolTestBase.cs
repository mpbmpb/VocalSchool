using Microsoft.EntityFrameworkCore;
using System;
using VocalSchool.Models;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace VocalSchool.Test.Infrastructure
{
    public class VocalSchoolTestBase : IDisposable
    {
        protected readonly SchoolContext _seedcontext;
        protected readonly SchoolContext _context;
        protected readonly SchoolContext _resultcontext;
        public bool isLazyLoading = true;

        public VocalSchoolTestBase()
        {
            var options = new DbContextOptionsBuilder<SchoolContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            if (isLazyLoading)
            {
                options = new DbContextOptionsBuilder<SchoolContext>()
                .UseLazyLoadingProxies()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            }
          
            _seedcontext = new SchoolContext(options);
            _context = new SchoolContext(options);
            _resultcontext = new SchoolContext(options);

            _context.Database.EnsureCreated();

            VocalSchoolInitializer.Initialize(_seedcontext);
        }

        public void Dispose()
        {
            _seedcontext.Database.EnsureDeleted();
            _context.Database.EnsureDeleted();
            _resultcontext.Database.EnsureDeleted();

            _seedcontext.Dispose();
            _context.Dispose();
            _resultcontext.Dispose();

        }
    }
}
