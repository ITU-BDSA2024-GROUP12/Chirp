using Chirp.Core;
using Chirp.Infrastructure;
using Chirp.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

//EF core database context setup
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<CheepDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//EF Identity
builder.Services.AddDefaultIdentity<ChirpUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<CheepDbContext>();

//Github OAuth authorization option
builder.Services.AddAuthentication(options =>
    {
    })
    .AddCookie()
    .AddGitHub(o =>
    {
        var clientId = builder.Configuration["authentication_github_clientId"];
        var clientSecret = builder.Configuration["authentication_github_clientSecret"];

        if (!string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret))
        {
            o.ClientId = clientId;
            o.ClientSecret = clientSecret;
        }
        o.CallbackPath = "/signin-github";
        o.Scope.Clear(); //clear default scope
        o.Scope.Add("user:email");
    });


//Taken from https://stackoverflow.com/questions/31886779/asp-net-mvc-6-aspnet-session-errors-unable-to-resolve-service-for-type
/*builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(1800);
    options.Cookie.HttpOnly = false;
    options.Cookie.IsEssential = true;
});*/


// Add services to the container.
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<ICheepRepository, CheepRepository>();
builder.Services.AddRazorPages();


// Create a disposable service scope
var app = builder.Build();
//code block taken from slides
using (var scope = app.Services.CreateScope())
{
    // From the scope, get an instance of our database context.
    // Through the `using` keyword, we make sure to dispose it after we are done.
    using CheepDbContext? context = scope.ServiceProvider.GetService<CheepDbContext>();
    // Execute the migration from code.
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseMigrationsEndPoint();
    }
    else
    {
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
        context?.Database.Migrate();
    }

    if (context is not null)
    {
        DbInitializer.SeedDatabase(context);
    }
    
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
//app.UseSession();

app.MapRazorPages();

app.Run();

//For testing https://stackoverflow.com/questions/55131379/integration-testing-asp-net-core-with-net-framework-cant-find-deps-json/70490057#70490057
public partial class Program { }
