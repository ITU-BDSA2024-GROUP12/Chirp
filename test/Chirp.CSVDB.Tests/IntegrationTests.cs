using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using SimpleDB;
using Chirp.CLI;

public class CSVDatabaseIntegrationTests : IDisposable
{
    private readonly string _originalFilePath = @"../../../../../src/Chirp.CLI/chirp_cli_db.csv";





    private readonly string _testFilePath;

    public CSVDatabaseIntegrationTests()
    {
        // Creates a copy of the csv for testing purposes
        _testFilePath = Path.Combine(Path.GetTempPath(), $"chirp_cli_db_{Guid.NewGuid()}.csv");
        File.Copy(_originalFilePath, _testFilePath);
    }
    //testing the read function of CSVDatabase
    [Fact]
    public void CSVDatabaseReadRecords()
    {
        // Arrange
        IDatabaseRepository<Cheep> database = new CSVDatabase<Cheep>(_testFilePath);

        // Act
        var cheepsRead = database.Read().ToList();

        // Assert
        //Assert.Equal(9, cheepsRead.Count);  not sure how sensible this test is
        Assert.Equal("ropf", cheepsRead[0].Author);
        Assert.Equal("Hello, BDSA students!", cheepsRead[0].Message);
        Assert.Equal(1690891760, cheepsRead[0].Timestamp);

        Assert.Equal("adho", cheepsRead[1].Author);
        Assert.Equal("Welcome to the course!", cheepsRead[1].Message);
        Assert.Equal(1690978778, cheepsRead[1].Timestamp);


        Assert.Equal("cmolu", cheepsRead[6].Author);
        Assert.Equal("does this work", cheepsRead[6].Message);
        Assert.Equal(1724848909, cheepsRead[6].Timestamp);
    }
    //testing writing arecord to the database and then retrieving it
    [Fact]
    public void CSVDatabaseStoreRecord()
    {
        //Arrange
        IDatabaseRepository<Cheep> database = new CSVDatabase<Cheep>(_testFilePath);
        var newCheep = new Cheep()
        {
            Author = Environment.UserName,
            Message = "Integration test 2 works!",
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()

        };

        var cheepsToStore = new List<Cheep> { newCheep };

        //Act
        database.Store(cheepsToStore);
        var cheepsRead = database.Read();


        //Assert
        Assert.Contains(cheepsRead, cheep =>
            cheep.Author == newCheep.Author &&
            cheep.Message == newCheep.Message &&
            cheep.Timestamp == newCheep.Timestamp);
    }

    // Cleanup: This deletes the temporary csv file
    public void Dispose()
    {
        if (File.Exists(_testFilePath))
        {
            File.Delete(_testFilePath);
        }
    }
}