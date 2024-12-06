using System.Threading.Channels;
using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace Chirp.RazorPages.Tests;

public class AuthorRepositoryTests
{
    private readonly DbContextOptionsBuilder<CheepDbContext> _builder;

    public AuthorRepositoryTests(ITestOutputHelper testOutputHelper)
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
    
    //CreateAuthor
    [Fact]
    public void CreateAuthorExistingAuthor()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var repository = new AuthorRepository(dbContext);
        var author = new Author {AuthorId = 999, Name = "Jane Test", Email = "jane@test.com" };
        dbContext.Authors.Add(author);
        dbContext.SaveChanges();

        var authorDto = new AuthorDTO {AuthorId = 999, Name = "Jane Test", Email = "jane@test.com" };

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => repository.CreateAuthor(authorDto));
        Assert.Equal("Author Jane Test already exists", exception.Message);
    }
    
    [Fact]
    public async Task CreateNonExistingAuthor()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var repository = new AuthorRepository(dbContext);
        var authorDto = new AuthorDTO {AuthorId = 999, Name = "John Testman", Email = "john@test.com" };

        // Act
        var result = repository.CreateAuthor(authorDto);

        // Assert
        Assert.True(result);
        var authorInDb = await dbContext.Authors.FirstOrDefaultAsync(a => a.Name == "John Testman");
        Assert.NotNull(authorInDb);
        Assert.Equal("john@test.com", authorInDb.Email);
    }
    
    [Theory]
    [InlineData("Adrian", "adho@itu.dk")]
    public async void CanCreateAuthor_ThrowsException(string author, string email)
    {
        // Arrange
        CheepDbContext context = new CheepDbContext(_builder.Options);
        await context.Database.EnsureCreatedAsync();

        // Applies the schema to the database
        DbInitializer.SeedDatabase(context);

        IAuthorRepository repository = new AuthorRepository(context);
        
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

        IAuthorRepository repository = new AuthorRepository(context);

        AuthorDTO authorDto = new AuthorDTO()
        {
            AuthorId = 999,
            Name = author,
            Email = email
        };

        // Act & Assert
        bool result = repository.CreateAuthor(authorDto);

        Assert.True(result);
    }

    //GetAuthorByEmail
    [Fact]
    public async Task GetAuthorByEmailWhenAuthorExists()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var repository = new AuthorRepository(dbContext);
        var author = new Author { AuthorId = 1, Name = "Jane Test", Email = "jane@test.com" };
        dbContext.Authors.Add(author);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await repository.GetAuthorByEmail("jane@test.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Jane Test", result.Name);
    }
    
    //GetAuthor
    [Theory]
    [InlineData("Johannes", "johje@itu.dk")]
    public async void GetAuthor(string author, string email)
    {
        // Arrange
        CheepDbContext context = new CheepDbContext(_builder.Options);
        await context.Database.EnsureCreatedAsync();
        
        // Applies the schema to the database
        DbInitializer.SeedDatabase(context);
        IAuthorRepository repository = new AuthorRepository(context);
        
        //Assert
        await Assert.ThrowsAsync<UserNotFoundException>(() => repository.GetAuthor(author,email));
    }

    //FollowUser()
    [Fact]
    public async void UserCanFollowAuthor()
    {
        // Arrange
        CheepDbContext dbContext = new CheepDbContext(_builder.Options);
        await dbContext.Database.EnsureCreatedAsync();
        IAuthorRepository repository = new AuthorRepository(dbContext);
        
        //Act
        AuthorDTO author1 = new AuthorDTO { AuthorId = 1, Name = "Jane Test", Email = "jane@test.com" };
        AuthorDTO author2 = new AuthorDTO { AuthorId = 2, Name = "John Test", Email = "john@test.com" };
        repository.CreateAuthor(author1);
        repository.CreateAuthor(author2);
        
        //This is weird...
        repository.FollowUser(author2.AuthorId, author1.Name);
        var ids = await repository.GetFollowerIds(author1.Name);

        //Assert
        Assert.Equal(author2.AuthorId, ids[0]);
    }
    
    [Fact]
    public async void UserCanFollowAuthorDouble(){
        // Arrange
        CheepDbContext dbContext = new CheepDbContext(_builder.Options);
        await dbContext.Database.EnsureCreatedAsync();
        IAuthorRepository repository = new AuthorRepository(dbContext);
        
        //Act
        AuthorDTO author1 = new AuthorDTO { AuthorId = 1, Name = "Jane Test", Email = "jane@test.com" };
        AuthorDTO author2 = new AuthorDTO { AuthorId = 2, Name = "John Test", Email = "john@test.com" };
        repository.CreateAuthor(author1);
        repository.CreateAuthor(author2);
        
        //This is weird...
        repository.FollowUser(author2.AuthorId, author1.Name);
        repository.FollowUser(author2.AuthorId, author1.Name);
        var ids = await repository.GetFollowerIds(author1.Name);

        //Assert
        Assert.Single(ids);
    }
    
    [Fact]
    public async void UserCanUnfollowAuthor(){
        // Arrange
        CheepDbContext dbContext = new CheepDbContext(_builder.Options);
        await dbContext.Database.EnsureCreatedAsync();
        IAuthorRepository repository = new AuthorRepository(dbContext);
        
        //Act
        AuthorDTO author1 = new AuthorDTO { AuthorId = 1, Name = "Jane Test", Email = "jane@test.com" };
        AuthorDTO author2 = new AuthorDTO { AuthorId = 2, Name = "John Test", Email = "john@test.com" };
        repository.CreateAuthor(author1);
        repository.CreateAuthor(author2);
        
        //This is weird...
        repository.FollowUser(author2.AuthorId, author1.Name);
        repository.UnfollowUser(author2.AuthorId, author1.Name);
        var ids = await repository.GetFollowerIds(author1.Name);

        //Assert
        Assert.Null(ids);
    }
}