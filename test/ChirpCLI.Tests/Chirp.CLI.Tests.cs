using System.IO;
using Xunit;
using Chirp.Interface;
using Chirp.CLI;

namespace ChirpCLITests;

public class ChirpCLITests
{
    [Fact]
    public void InterfaceTest()
    {
        //Arrange--
        //as it is a static class it doesn't need to be instantiated
        List<Cheep> records = new List<Cheep>{
		new Cheep() { Author = "person", Message = "This is a message", Timestamp = 1726056651 }
		};//("person", "This is a message", 1726056651); //Wed Sep 11 14:10:51 2024 CEST
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter); //to capture the printed message
        
        //Act--
        UserInterface.PrintCheeps(records);
		
        //Assert
        var output = stringWriter.ToString();
        Assert.Equal("person @ 09/11/2024 14:10:51: This is a message\n", output);

    }
}