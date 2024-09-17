using System.IO;
using System.Reflection;
using Xunit;
using Chirp.Interface;
using Chirp.CLI;

namespace ChirpCLITests;

public class ChirpInterfaceTests
{
    [Fact]
    public void InterfaceTest1() //testing the bare minimum of the PrintCheeps method
    {
        //Arrange--
        //as it is a static class it doesn't need to be instantiated
        List<Cheep> records = new List<Cheep>{
		new Cheep() { Author = "person", Message = "This is a message", Timestamp = 1726056651 }//Wed Sep 11 14:10:51 2024 CEST
		}; 
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter); //to capture the printed message
        
        //Act--
        UserInterface.PrintCheeps(records);
		
        //Assert
        var output = stringWriter.ToString();
        Assert.Equal("person @ " + new DateTime(2024, 09, 11, 14, 10 ,51) + ": This is a message\n", output, ignoreLineEndingDifferences: true); //compares commandline output
    }

[Fact]
    public void InterfaceTest2() //testing printing multiple cheep prints
    {
		//reason for not using theory with multiple testcases, is that here is tested a single case with multiple Cheeps.

        //Arrange--
        List<Cheep> records = new List<Cheep>{
		new Cheep() { Author = "person1", Message = "This is the first message", Timestamp = 1726167611 },//Thu Sep 12 21:00:11 2024 CEST
		new Cheep() { Author = "person2", Message = "This is the second message", Timestamp = 1726167639 },//Thu Sep 12 21:00:39 2024 CEST
		new Cheep() { Author = "person3", Message = "This is a multiline message \n second line", Timestamp = 1726168068 }//Thu Sep 12 21:07:48 2024 CEST
		}; 
        DateTime time1 = new DateTime(2024, 09, 12, 21, 00, 11), time2 = new DateTime(2024, 09, 12, 21, 00, 39), time3 = new DateTime(2024, 09, 12, 21, 07, 48);
		var expectedOutput = String.Format("person1 @ "+ time1 +": This is the first message\nperson2 @ "+ time2 +": This is the second message\nperson3 @ "+ time3 +": This is a multiline message \n second line\n");
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter); //to capture the printed message
        
        //Act--
        UserInterface.PrintCheeps(records);
		
        //Assert
        var output = stringWriter.ToString();
        Assert.Equal(expectedOutput, output, ignoreLineEndingDifferences: true); //compares commandline output
    }

	//Testing the UnixtTime conversion of UnixTimeStampToDateTime in UserInterface.
	[Theory]
	[MemberData(nameof(DateTimeTestData))] //MemberData is used instead of inline due to use of DateTime object
	public void UnixTimeStampToDateTimeTest(int unixTime, DateTime expected)
	{
		//arrange
		//Reflection is used as desired method is private and therefore inaccesaible.
		Type type = typeof(UserInterface); 
		MethodInfo method = type.GetMethod("UnixTimeStampToDateTime", BindingFlags.NonPublic | BindingFlags.Static);

		//act
		DateTime output = (DateTime)method.Invoke(null, new object[] {unixTime});

		//Assert
		Assert.Equal(expected, output);
	}

	public static IEnumerable<object[]> DateTimeTestData() //data generator.
	{
		yield return new object[] {1726309240, new DateTime(2024, 09, 14, 12, 20, 40)};
		yield return new object[] {1654669098, new DateTime(2022, 06, 08, 08, 18, 18)};
		yield return new object[] {325067400, new DateTime(1980, 04, 20, 10, 30, 00)};
	

	}
}