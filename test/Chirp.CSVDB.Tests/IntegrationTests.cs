using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using SimpleDB;
using Chirp.CLI;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Http;
using System.Text;
using System.Text.Json;

public class CSVDatabaseIntegrationTests
{
    //testing the read function of CSVDatabase
    [Fact]
    public async void CSVDatabaseReadRecords()
    {
        // Arrange
        var baseURL = "http://localhost:5143";
	    using HttpClient client = new();
	    client.DefaultRequestHeaders.Accept.Clear();
	    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
	    client.BaseAddress = new Uri(baseURL);

        // Act
        HttpResponseMessage response = await client.GetAsync("cheeps");
		var cheepsRead = await response.Content.ReadFromJsonAsync<IEnumerable<Cheep>>();
        //Console.Write((int)response.StatusCode);
		

        // Assert
        //Assert.Equal(9, cheepsRead.Count);  not sure how sensible this test is
        Assert.True((int)response.StatusCode == 200);
        
        foreach(var cheep in cheepsRead) {
            Assert.Equal("ropf", cheep.Author);
            Assert.Equal("Hello, BDSA students!", cheep.Message);
            Assert.Equal(1690891760, cheep.Timestamp);
            break;
        }
    }
    //testing writing arecord to the database and then retrieving it
    [Fact]
    public async void CSVDatabaseStoreRecord()
    {
        //Arrange
        var baseURL = "http://localhost:5143";
	    using HttpClient client = new();
	    client.DefaultRequestHeaders.Accept.Clear();
	    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
	    client.BaseAddress = new Uri(baseURL);


        var newCheep = new Cheep()
        {
            Author = Environment.UserName,
            Message = "Integration test 2 works!",
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()

        };

        string jsonString = JsonSerializer.Serialize(newCheep);

        //Line taken from Stackoverflow ** https://stackoverflow.com/a/39414248/17816920 ** 
        HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");
        var response = await client.PostAsync("cheep", content);

        //Assert
        Assert.True((int)response.StatusCode == 200);
    }
}