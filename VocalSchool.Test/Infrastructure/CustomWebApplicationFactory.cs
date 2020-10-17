using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VocalSchool.Data;
using VocalSchool.Models;

namespace VocalSchool.Test.Infrastructure
{

        public class CustomWebApplicationFactory<TStartup>
            : WebApplicationFactory<TStartup> where TStartup: class
        {
            private bool _isLazyLoading;
            
            protected override void ConfigureWebHost(IWebHostBuilder builder)
            {
                using (var fixture = new ConfigFixture())
                {
                    _isLazyLoading = fixture.IsLazyLoading;
                }
                
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType ==
                             typeof(DbContextOptions<SchoolContext>));

                    services.Remove(descriptor);

                    var name = Guid.NewGuid().ToString();

                    services.AddDbContext<SchoolContext>(options =>
                    {
                        options.UseInMemoryDatabase(name).UseLazyLoadingProxies(_isLazyLoading);
                    });

                    var sp = services.BuildServiceProvider();

                    
                    using (var scope = sp.CreateScope())
                    {
                        var scopedServices = scope.ServiceProvider;
                        var dbContext = scopedServices.GetRequiredService<SchoolContext>();
                        var logger = scopedServices
                            .GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                        dbContext.Database.EnsureCreated();

                        DbInitializer.Initialize(dbContext);
                    }
                });
            }
        }

}