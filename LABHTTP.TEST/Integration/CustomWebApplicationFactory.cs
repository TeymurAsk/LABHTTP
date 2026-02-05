using LABHTTP.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LABHTTP.TEST.Integration
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                // Use test-specific config with test database
                config.AddJsonFile("appsettings.Test.json");
            });

            builder.ConfigureServices(services =>
            {
                // Create a service provider to resolve the DB
                var sp = services.BuildServiceProvider();

                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // Ensure database is clean
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            });
        }


        protected override void ConfigureClient(HttpClient client)
        {
            client.BaseAddress = new Uri("https://localhost");
        }

    }
}
