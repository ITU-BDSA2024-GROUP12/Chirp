namespace Chirp.Core.Tests;


public class UnitTest
{
    //Author DTO
    [Fact]
    public void AuthorDTOtest()
    {
        //Arrange
        AuthorDTO authorDto = new AuthorDTO()
        {
            AuthorId = 1,
            Email = "test@test.com",
            Name = "test"
        };
        //Act & Asert
        
        
        Assert.Equal(1, authorDto.AuthorId);
        Assert.Equal("test@test.com", authorDto.Email);
        Assert.Equal("test", authorDto.Name);
    }
    
    [Fact]
    public void AuthorDTOtypes()
    {
        //Arrange
        AuthorDTO authorDto = new AuthorDTO()
        {
            AuthorId = 1,
            Email = "test@test.com",
            Name = "test"
        };
        //Act & Asert
        
        Assert.IsType(typeof(int), authorDto.AuthorId);
        Assert.IsType(typeof(string), authorDto.Email);
        Assert.IsType(typeof(string), authorDto.Name);
        
    }
    
    //CheepDTO
}