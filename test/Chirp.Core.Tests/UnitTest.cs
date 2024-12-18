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
        
        Assert.IsType<int>(authorDto.AuthorId);
        Assert.IsType<string>(authorDto.Email);
        Assert.IsType<string>(authorDto.Name);
    }
    
    //CheepDTO
    [Fact]
    public void CheepDTOtest()
    {
        //Arrange
        CheepDTO cheepDto = new CheepDTO()
        {
            Author = "test",
            Text = "test",
            TimeStamp = 0
        };
        //Act & Asert
        
        
        Assert.Equal("test", cheepDto.Author);
        Assert.Equal("test", cheepDto.Text);
        Assert.Equal(0, cheepDto.TimeStamp);
    }
    
    [Fact]
    public void CheepDTOtypes()
    {
        //Arrange
        CheepDTO cheepDto = new CheepDTO()
        {
            Author = "test",
            Text = "test",
            TimeStamp = 0
        };
        //Act & Asert
        
        Assert.IsType<long>(cheepDto.TimeStamp);
        Assert.IsType<string>(cheepDto.Author);
        Assert.IsType<string>(cheepDto.Text);
    }
}