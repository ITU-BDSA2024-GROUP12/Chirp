using System.Globalization;
using System.Text.RegularExpressions;
using Chirp.Interface;
using DocoptNet;
using CsvHelper;
using CsvHelper.Configuration;
using SimpleDB;
using Chirp.CLI;


try
{
    bool IsWindows = System.OperatingSystem.IsWindows();
    const string Mode = @"Chirp CLI.

    Usage:
    chirp cheep <message>
    chirp read
    ";
    var arguments = new Docopt().Apply(Mode, args, exit: true);
    
    
    // check if os is windows
    string filepath;
    if (IsWindows)
    {
        filepath = new(@".\chirp_cli_db.csv");
    }
    else
    {
        filepath = new(@"./chirp_cli_db.csv");
    }
    
    //Initialize the cheep CSVDatabase interface
    IDatabaseRepository<Cheep> database = CSVDatabase<Cheep>.GetInstance(filepath);

    
    if (arguments["cheep"].IsTrue)
    {
        var message = arguments["<message>"].ToString();
        var user = Environment.UserName;
        DateTime currentTime = DateTime.UtcNow;
        long unixTime = ((DateTimeOffset)currentTime).ToUnixTimeSeconds();


        var records = new List<Cheep>
        {
            new Cheep() { Author = user, Message = message, Timestamp = unixTime },
        };
        
        database.Store(records);
    }
    else if (arguments["read"].IsTrue)
    {
        // Open the text file using a stream reader.
        using (var reader = new StreamReader(filepath))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            IEnumerable<Cheep> records = database.Read();
            UserInterface.PrintCheeps(records); //Prints cheeps using static Userinterface
        }
    }
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
} 

namespace Chirp.CLI{

	public class Cheep
	{
    	public string Author { get; set; }
    	public string Message { get; set; }
    	public long Timestamp { get; set; }
	}
}

