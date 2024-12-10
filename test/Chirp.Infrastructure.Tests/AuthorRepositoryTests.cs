using System.Threading.Channels;
using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace Chirp.Infrastructure.Tests;

public class AuthorRepositoryTests
{
    private readonly DbContextOptionsBuilder<CheepDbContext> _builder;

    public AuthorRepositoryTests(ITestOutputHelper testOutputHelper)
    {
        SqliteConnection connection = new SqliteConnection("Filename=:memory:");
        connection.OpenAsync();
        _builder = new DbContextOptionsBuilder<CheepDbContext>().UseSqlite(connection);
        _builder.EnableSensitiveDataLogging();
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
    public async Task CreateAuthorExistingAuthor()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var repository = new AuthorRepository(dbContext);
        var author = new Author {AuthorId = 999, Name = "Jane Test", Email = "jane@test.com" };
        await dbContext.Authors.AddAsync(author);
        await dbContext.SaveChangesAsync();

        var authorDto = new AuthorDTO {AuthorId = 999, Name = "Jane Test", Email = "jane@test.com" };

        // Act & Assert
        var tsk = await Assert.ThrowsAsync<Exception>( async () => await repository.CreateAuthor(authorDto));
        Assert.Equal("Author Jane Test already exists", tsk.Message);
    }
    
    [Fact]
    public async Task CreateNonExistingAuthor()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var repository = new AuthorRepository(dbContext);
        var authorDto = new AuthorDTO {AuthorId = 999, Name = "John Testman", Email = "john@test.com" };

        // Act
        var result = await repository.CreateAuthor(authorDto);

        // Assert
        Assert.Equal(1,result);
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
       await Assert.ThrowsAsync<Exception>(async () => await repository.CreateAuthor(authorDto));
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
        var result = await repository.CreateAuthor(authorDto);

        Assert.Equal(1,result);
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
        await repository.CreateAuthor(author1);
        await repository.CreateAuthor(author2);
        
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
        await repository.CreateAuthor(author1);
        await repository.CreateAuthor(author2);
        
        
        repository.FollowUser(author2.AuthorId, author1.Name);
        

        //Assert
        var tsk = Assert.Throws<Exception>(() => repository.FollowUser(author2.AuthorId, author1.Name));
        Assert.Equal("Author Jane Test is already being followed!", tsk.Message);
    }
    
    [Fact]
    public async Task UserCanUnfollowAuthor(){
        // Arrange
        CheepDbContext dbContext = new CheepDbContext(_builder.Options);
        await dbContext.Database.EnsureCreatedAsync();
        IAuthorRepository repository = new AuthorRepository(dbContext);
        
        //Act
        AuthorDTO author1 = new AuthorDTO { AuthorId = 1, Name = "User1", Email = "user1@test.com" };
        AuthorDTO author2 = new AuthorDTO { AuthorId = 2, Name = "User2", Email = "user2@test.com" };
        await repository.CreateAuthor(author1);
        await repository.CreateAuthor(author2);
        
        repository.FollowUser(author2.AuthorId, author1.Name);
        await repository.UnfollowUser(author2.AuthorId, author1.Name);
        var ids = await repository.GetFollowerIds(author1.Name);

        //Assert
        Assert.Empty(ids);
    }
    
    [Fact]
    public async Task UserCanUnfollowNonFollowedAuthor(){
        // Arrange
        CheepDbContext dbContext = new CheepDbContext(_builder.Options);
        await dbContext.Database.EnsureCreatedAsync();
        IAuthorRepository repository = new AuthorRepository(dbContext);
        
        //Act
        AuthorDTO author1 = new AuthorDTO { AuthorId = 1, Name = "User1", Email = "user1@test.com" };
        AuthorDTO author2 = new AuthorDTO { AuthorId = 2, Name = "User2", Email = "user2@test.com" };
        await repository.CreateAuthor(author1);
        await repository.CreateAuthor(author2);
        
        await repository.UnfollowUser(author2.AuthorId, author1.Name);
        var ids = await repository.GetFollowerIds(author1.Name);

        //Assert
        Assert.Empty(ids);

    }
}