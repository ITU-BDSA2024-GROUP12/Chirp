using System.Globalization;
using System.Text.RegularExpressions;
using DocoptNet;
using CsvHelper;

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

        using (StreamWriter sw = File.AppendText(path))
        {
            sw.WriteLine($"{user},\"{message}\",{unixTime}");
            sw.Close();
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
            var records = csv.GetRecords<Cheep>();

            foreach (var lines in records)
            {
                DateTime time = UnixTimeStampToDateTime(lines.Timestamp);
                Console.WriteLine($"{lines.Author} @ {time}: {lines.Message}");
            }
        }
    }
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
} 

DateTime UnixTimeStampToDateTime( double unixTimeStamp ) // Taken from stackoverflow https://stackoverflow.com/questions/249760/how-can-i-convert-a-unix-timestamp-to-datetime-and-vice-versa
{
    DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    dateTime = dateTime.AddSeconds( unixTimeStamp ).ToLocalTime();
    return dateTime;
}

public class Cheep
{
    public string Author { get; set; }
    public string Message { get; set; }
    public long Timestamp { get; set; }
}