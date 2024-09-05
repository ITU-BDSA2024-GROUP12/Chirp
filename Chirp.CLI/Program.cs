using System.Text.RegularExpressions;
using DocoptNet;
try
{	bool IsWindows = System.OperatingSystem.IsWindows();
    const string Mode = @"Chirp CLI.

    Usage:
    chirp cheep <message>
    chirp read
    ";
    var arguments = new Docopt().Apply(Mode, args, exit: true);
    if (arguments["cheep"].IsTrue)
    {	string path;
        var message = arguments["<message>"].ToString();
        var user = Environment.UserName;
        DateTime currentTime = DateTime.UtcNow;
        long unixTime = ((DateTimeOffset)currentTime).ToUnixTimeSeconds();
        
        if (IsWindows){
			path = new(@".\chirp_cli_db.csv");
		} else {
			path = new(@"./chirp_cli_db.csv");
		}
		
        using (StreamWriter sw = File.AppendText(path))
        {
            sw.WriteLine($"{user},\"{message}\",{unixTime}");
            sw.Close();
        }	

    } else if (arguments["read"].IsTrue)
    {
        List<string[]> cheeps = new List<string[]>();
		string filepath;
        // Open the text file using a stream reader.
        if (IsWindows){
			filepath = new(@".\chirp_cli_db.csv");
		} else {
			filepath = new(@"./chirp_cli_db.csv");
		}
		using StreamReader reader = new(filepath);
        {
            string line; 
            while ((line = reader.ReadLine()) != null)
            {
                //Define pattern
                Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))"); // regex taken from this post on stackoverflow https://stackoverflow.com/questions/3507498/reading-csv-files-using-c-sharp/34265869#34265869

                //Separating columns to array
                string[] X = CSVParser.Split(line);
                cheeps.Add(X);
            }
        }
        cheeps.RemoveAt(0);
        foreach (var lines in cheeps)
        {
            string message = lines[1];
            string author = lines[0];
            DateTime time = UnixTimeStampToDateTime(Double.Parse(lines[2]));
            Console.WriteLine($"{author} @ {time}: {message}");
        }
    }
}
catch (Exception e)
{ } 

DateTime UnixTimeStampToDateTime( double unixTimeStamp ) // Taken from stackoverflow https://stackoverflow.com/questions/249760/how-can-i-convert-a-unix-timestamp-to-datetime-and-vice-versa
{
    DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    dateTime = dateTime.AddSeconds( unixTimeStamp ).ToLocalTime();
    return dateTime;
}