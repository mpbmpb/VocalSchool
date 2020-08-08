using Microsoft.EntityFrameworkCore;
using System;
using VocalSchool.Models;

namespace VocalSchool.Test.Infrastructure
{
    public class VocalSchoolTestBase : IDisposable
    {
        protected readonly SchoolContext _context;

        public VocalSchoolTestBase()
        {
            var options = new DbContextOptionsBuilder<SchoolContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SchoolContext(options);

            _context.Database.EnsureCreated();

            VocalSchoolInitializer.Initialize(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();

            _context.Dispose();
        }
    }
}
