using System.Globalization;
using System.Text.RegularExpressions;
using Chirp.CLI;
using DocoptNet;
using CsvHelper;
using CsvHelper.Configuration;

try
{
    bool IsWindows = System.OperatingSystem.IsWindows();
    const string Mode = @"Chirp CLI.

    Usage:
    chirp cheep <message>
    chirp read
    ";
    var arguments = new Docopt().Apply(Mode, args, exit: true);
    if (arguments["cheep"].IsTrue)
    {
        string path;
        var message = arguments["<message>"].ToString();
        var user = Environment.UserName;
        DateTime currentTime = DateTime.UtcNow;
        long unixTime = ((DateTimeOffset)currentTime).ToUnixTimeSeconds();

        if (IsWindows)
        {
            path = new(@".\chirp_cli_db.csv");
        }
        else
        {
            path = new(@"./chirp_cli_db.csv");
        }
        
        var records = new List<Cheep>
        {
            new Cheep() { Author = user, Message = message, Timestamp = unixTime},
        };
        
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            // Don't write the header again.
            HasHeaderRecord = false,
        };
        using (var stream = File.Open(path, FileMode.Append))
        using (var writer = new StreamWriter(stream))
        using (var csv = new CsvWriter(writer, config))
        {
            csv.WriteRecords(records);
        }
    }
    else if (arguments["read"].IsTrue)
    {
        string filepath;
        // Open the text file using a stream reader.
        if (IsWindows)
        {
            filepath = new(@".\chirp_cli_db.csv");
        }
        else
        {
            filepath = new(@"./chirp_cli_db.csv");
        }

        using (var reader = new StreamReader(filepath))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            UserInterface.PrintCheeps(csv.GetRecords<Cheep>()); //Prints cheeps using static Userinterface
        }
    }
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
} 


public class Cheep
{
    public string Author { get; set; }
    public string Message { get; set; }
    public long Timestamp { get; set; }
}