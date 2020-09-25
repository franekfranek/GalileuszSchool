using GalileuszSchool.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalileuszSchool.Tests.IntegrationTests
{
    public class TestingWebAppFactory<T> : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                  d => d.ServiceType ==
                     typeof(DbContextOptions<GalileuszSchoolContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }
                //Entity Framework in-memory database support to the DI container
                var serviceProvider = new ServiceCollection()
                  .AddEntityFrameworkInMemoryDatabase()
                  .BuildServiceProvider();
                //this is in memory db call can be done to real sql server one
                services.AddDbContext<GalileuszSchoolContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryCalendarEventsTest");
                    options.UseInternalServiceProvider(serviceProvider);
                });
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<GalileuszSchoolContext>();
                    db.Database.EnsureCreated();

                    try
                    {
                        Helpers.Utilities.InitializeDbForTests(db);

                    }
                    catch (Exception ex)
                    {
                        //Log errors 
                        throw ex;
                    }
                    }
                
            });
        }
    }
}
