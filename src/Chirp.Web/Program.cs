using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

//EF core database context setup
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CheepDbContext>(options => options.UseSqlite(connectionString));

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddScoped<ICheepRepository, CheepRepository>();

// Create a disposable service scope
var app = builder.Build();
//code block taken from slides
using (var scope = app.Services.CreateScope())
{
    // From the scope, get an instance of our database context.
    // Through the `using` keyword, we make sure to dispose it after we are done.
    using var context = scope.ServiceProvider.GetService<CheepDbContext>();
    // Execute the migration from code.
    context.Database.Migrate();
    
    DbInitializer.SeedDatabase(context);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();

app.Run();
