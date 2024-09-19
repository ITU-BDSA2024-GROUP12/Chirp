﻿using Chirp.CLI;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

string path;
bool IsWindows = System.OperatingSystem.IsWindows();
if (IsWindows)
{
    path = new(@"..\Chirp.CLI\chirp_cli_db.csv");
}
else
{
    path = new(@"../Chirp.CLI/chirp_cli_db.csv");
}

app.MapGet("/cheeps", () =>
{
    //READ
    var records = new List<Cheep>();
    using (var reader = new StreamReader(path))
    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
    {
        records = csv.GetRecords<Cheep>().ToList();

        return records;
    }
});

app.MapPost("/cheep", (Cheep cheep) =>
{
    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
    {
        // Don't write the header again.
        HasHeaderRecord = false,
    };
    using (var stream = File.Open(path, FileMode.Append))
    using (var writer = new StreamWriter(stream))
    using (var csv = new CsvWriter(writer, config))
    {
        csv.WriteRecord(cheep);
    }
});

app.Run();