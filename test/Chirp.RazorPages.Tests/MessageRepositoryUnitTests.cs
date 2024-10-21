using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Chirp.RazorPages.Tests;

public class MessageRepositoryUnitTests
{
    public MessageRepositoryUnitTests()
    {
        
    }

    [Fact]
    public async void test()
    {
        //Taken from slides: https://github.com/itu-bdsa/lecture_notes/blob/main/sessions/session_07/Slides.md
        using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<CheepDbContext>().UseSqlite(connection);

        using var context = new CheepDbContext(builder.Options);
        await context.Database.EnsureCreatedAsync(); // Applies the schema to the database

        DbInitializer.SeedDatabase(context); //empty database otherwise? 
        
        ICheepRepository repository = new CheepRepository(context);
        
        List<CheepDTO> list = await repository.ReadMessagesFromAuthor("Helge", 1);
        Assert.NotEmpty(list);
    }
}