using System.Globalization;
using System.Text.RegularExpressions;
using Chirp.Interface;
using DocoptNet;
using CsvHelper;
using CsvHelper.Configuration;
using SimpleDB;
using Chirp.CLI;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Net.Http;
using System.Text;
using System.Text.Json;


try
{
    bool IsWindows = System.OperatingSystem.IsWindows();
    const string Mode = @"Chirp CLI.

    Usage:
    chirp cheep <message>
    chirp read
    ";
    var arguments = new Docopt().Apply(Mode, args, exit: true);

    /* Code Taken from session 4 slides*/
    var baseURL = "http://bdsagroup12chirpremotedb.azurewebsites.net";
	using HttpClient client = new();
	client.DefaultRequestHeaders.Accept.Clear();
	client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
	client.BaseAddress = new Uri(baseURL);
    
    if (arguments["cheep"].IsTrue)
    {
        var message = arguments["<message>"].ToString();
        var user = Environment.UserName;
        DateTime currentTime = DateTime.UtcNow;
        long unixTime = ((DateTimeOffset)currentTime).ToUnixTimeSeconds();

        Cheep cheep = new Cheep() 
        {
            Author = user, 
            Message = message, 
            Timestamp = unixTime
        };
        
        string jsonString = JsonSerializer.Serialize(cheep);

        //Line taken from Stackoverflow ** https://stackoverflow.com/a/39414248/17816920 ** 
        HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");
        var response = await client.PostAsync("cheep", content);
        Console.Write(response);
        
    }
    else if (arguments["read"].IsTrue)
    {

        HttpResponseMessage response = await client.GetAsync("cheeps");
		IEnumerable<Cheep> records = await response.Content.ReadFromJsonAsync<IEnumerable<Cheep>>();
        
        //Console.Write((int)response.StatusCode);
		UserInterface.PrintCheeps(records);	
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

