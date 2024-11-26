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
        
        Assert.True(authorDto.AuthorId == 1);
        Assert.True(authorDto.Email == "test@test.com");
        Assert.True(authorDto.Name == "test");
    }
    
    //CheepDTO
}