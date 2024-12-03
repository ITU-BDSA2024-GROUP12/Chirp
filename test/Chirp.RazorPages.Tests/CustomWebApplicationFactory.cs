using Chirp.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public class CustomWebApplicationFactory<Program> : WebApplicationFactory<Program> where Program : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration.
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<CheepDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add an in-memory database for testing.
            services.AddDbContext<CheepDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });

			
            using (var scope = services.BuildServiceProvider().CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<CheepDbContext>();
                //**
                //CURRENTLY EMPTY DATABASE; CAN BE SEEDED IF UNCOMMENTED.
                //**
                //DbInitializer.SeedDatabase(db);
            }
        });
    }
}