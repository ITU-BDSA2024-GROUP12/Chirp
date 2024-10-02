using Chirp.SQLite;
using Microsoft.EntityFrameworkCore;
using DataModel;
var builder = WebApplication.CreateBuilder(args);

//EF core database context setup
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CheepDbContext>(options => options.UseSqlite(connectionString));

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSingleton<ICheepService, CheepService>();
/*builder.Services.AddSingleton<ICheepService>(provider =>
{
    // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-8.0
    // Also some Rider autofinish, until it worked.
    
    //maybe dependency injection? i don't actually know if this is how you its supposed to work. 
    IDatabaseRepository<CheepViewModel.CheepViewModel> database =
        DBFacade<CheepViewModel.CheepViewModel>.getInstance();
    
    return new CheepService(database);
});*/

var app = builder.Build();

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
