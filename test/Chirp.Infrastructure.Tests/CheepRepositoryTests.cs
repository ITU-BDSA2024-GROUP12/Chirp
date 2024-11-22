using System.Text;
using Chirp.Core;
using Chirp.Infrastructure;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

using System.Threading.Tasks;

namespace Chirp.RazorPages.Tests;

public class CheepRepositoryTests
{
    private readonly DbContextOptionsBuilder<CheepDbContext> _builder;

    public CheepRepositoryTests(ITestOutputHelper testOutputHelper)
    {
        SqliteConnection connection = new SqliteConnection("Filename=:memory:");
        connection.OpenAsync();
        _builder = new DbContextOptionsBuilder<CheepDbContext>().UseSqlite(connection);
    }

    private CheepDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<CheepDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new CheepDbContext(options);
    }
    
    //CreateCheep
    
    //CreateAuthor
    
    //DoesAuthorExist
    
    //GetAuthorByEmail
    
    //GetAuthorByName
    
    //GetAuthor
    
    //ReadMessage
    
    
    
    
    [Theory]
    [InlineData("Adrian", "adho@itu.dk")]
    public async void CanCreateAuthor_ThrowsException(string author, string email)
    {
        // Arrange
        CheepDbContext context = new CheepDbContext(_builder.Options);
        await context.Database.EnsureCreatedAsync();

        // Applies the schema to the database
        DbInitializer.SeedDatabase(context);

        ICheepRepository repository = new CheepRepository(context);
        
        AuthorDTO authorDto = new AuthorDTO()
        {
            Name = author,
            Email = email
        };

        // Act & Assert
        Assert.Throws<Exception>(() => repository.CreateAuthor(authorDto));
    }

    [Theory]
    [InlineData("Johannes", "johje@itu.dk")]
    public async void CanCreateAuthor(string author, string email)
    {
        // Arrange
        CheepDbContext context = new CheepDbContext(_builder.Options);
        await context.Database.EnsureCreatedAsync();

        // Applies the schema to the database
        DbInitializer.SeedDatabase(context);

        ICheepRepository repository = new CheepRepository(context);

        AuthorDTO authorDto = new AuthorDTO()
        {
            Name = author,
            Email = email
        };

        // Act & Assert
        bool result = repository.CreateAuthor(authorDto);

        Assert.True(result);
    }

    
    [Theory]
    [InlineData("John John","jojo@itu.dk","Hi everyone!, this is a test!","2024-10-22 13:27:18")]
    [InlineData("Adrian","adho@itu.dk","This is starting to look like something...","2024-10-22 13:34:53")]
    public async void CanCreateCheep(string author, string email, string text, string time)
    {
        // Arrange
        CheepDbContext context = new CheepDbContext(_builder.Options);
        await context.Database.EnsureCreatedAsync();
        
        // Applies the schema to the database
        DbInitializer.SeedDatabase(context);
        ICheepRepository repository = new CheepRepository(context);
        
        AuthorDTO authorDto = new AuthorDTO()
        {
            Name = author,
            Email = email
        };
        
        // Act
        repository.CreateCheep(authorDto, text, time);
        
        
        //Assert
        List<CheepDTO> result = repository.GetMessagesFromAuthor(author,0).Result;
        
        Assert.Equal(result.First().Text,text);
    }
    
    [Theory]
    [InlineData("Johannes", "johje@itu.dk")]
    public async void GetAuthor(string author, string email)
    {
        // Arrange
        CheepDbContext context = new CheepDbContext(_builder.Options);
        await context.Database.EnsureCreatedAsync();
        
        // Applies the schema to the database
        DbInitializer.SeedDatabase(context);
        ICheepRepository repository = new CheepRepository(context);
        
        await Assert.ThrowsAsync<UserNotFoundException>(() => repository.GetAuthor(author,email));
    }

    [Fact]
    public async Task CannotCreateCheepOver160()
    {
        // Arrange
        var author = "test";
        var email = "test@test.com";

        var sb = new StringBuilder();
        for (var i = 0; i < 161; i++)
        {
            sb.Append('.');
        }

        var text = sb.ToString();
            
        CheepDbContext context = new CheepDbContext(_builder.Options);
        await context.Database.EnsureCreatedAsync();
        
        // Applies the schema to the database
        DbInitializer.SeedDatabase(context);
        ICheepRepository repository = new CheepRepository(context);
        
        AuthorDTO authorDto = new AuthorDTO()
        {
            Name = author,
            Email = email
        };
        // Act & Assert
        Assert.Throws<InvalidDataException>(() => repository.CreateCheep(authorDto, text, DateTime.Now.ToString()));
    }
    
    [Fact]
    public async Task CreateNonExistingAuthor()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var repository = new CheepRepository(dbContext);
        var authorDto = new AuthorDTO { Name = "John Testman", Email = "john@test.com" };

        // Act
        var result = repository.CreateAuthor(authorDto);

        // Assert
        Assert.True(result);
        var authorInDb = await dbContext.Authors.FirstOrDefaultAsync(a => a.Name == "John Testman");
        Assert.NotNull(authorInDb);
        Assert.Equal("john@test.com", authorInDb.Email);
    }

    [Fact]
    public void CreateAuthorExistingAuthor()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var repository = new CheepRepository(dbContext);
        var author = new Author { Name = "Jane Test", Email = "jane@test.com" };
        dbContext.Authors.Add(author);
        dbContext.SaveChanges();

        var authorDto = new AuthorDTO { Name = "Jane Test", Email = "jane@test.com" };

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => repository.CreateAuthor(authorDto));
        Assert.Equal("Author Jane Test already exists", exception.Message);
    }

    [Fact]
    public async Task GetAuthorByEmailWhenAuthorExists()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var repository = new CheepRepository(dbContext);
        var author = new Author { AuthorId = 1, Name = "Jane Test", Email = "jane@test.com" };
        dbContext.Authors.Add(author);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await repository.GetAuthorByEmail("jane@test.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Jane Test", result.Name);
    }
    /*needs bugfix
    [Fact]
    public async Task GetAuthorByEmailNonExistingAuthor()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var repository = new CheepRepository(dbContext);

        // Act & Assert
        await Assert.ThrowsAsync<UserNotFoundException>(() => repository.GetAuthorByEmail("nonexistent@test.com"));
    }*/
    [Fact]
    public async Task CreateCheepWhenAuthorExists()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var repository = new CheepRepository(dbContext);
        var author = new Author { AuthorId = 1, Name = "John Testman", Email = "john@test.com" };
        dbContext.Authors.Add(author);
        await dbContext.SaveChangesAsync();

        var authorDto = new AuthorDTO { Name = "John Testman", Email = "john@test.com" };

        // Act
        var result = repository.CreateCheep(authorDto, "Hello World!", "2024-11-05 12:00:00");

        // Assert
        Assert.True(result);
        var cheepInDb = await dbContext.Cheeps.FirstOrDefaultAsync(c => c.Text == "Hello World!");
        Assert.NotNull(cheepInDb);
        Assert.Equal(author.AuthorId, cheepInDb.AuthorId);
    }

    [Fact]
    public async Task ReadMessageShouldReturnMessagesPaged()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var repository = new CheepRepository(dbContext);
        var author = new Author { Name = "John Testman" };
        dbContext.Authors.Add(author);

        dbContext.Cheeps.Add(new Cheep { Author = author, Text = "Hello World!", TimeStamp = DateTime.UtcNow });
        await dbContext.SaveChangesAsync();

        // Act
        var result = await repository.GetMessages(1);

        // Assert
        Assert.Single(result);
        Assert.Equal("Hello World!", result[0].Text);
    }

    [Fact]
    public async Task ReadMessagesFromAuthorPaged()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var repository = new CheepRepository(dbContext);
        var author = new Author { Name = "John Testman" };
        dbContext.Authors.Add(author);

        dbContext.Cheeps.Add(new Cheep { Author = author, Text = "Hello from John!", TimeStamp = DateTime.UtcNow });
        await dbContext.SaveChangesAsync();

        // Act
        var result = await repository.GetMessagesFromAuthor("John Testman", 1);

        // Assert
        Assert.Single(result);
        Assert.Equal("Hello from John!", result[0].Text);
    }
}