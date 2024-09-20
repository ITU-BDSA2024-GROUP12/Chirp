using Chirp.CLI;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.UseDeveloperExceptionPage();
app.UseStaticFiles();
app.UseRouting();

var filePath = Path.Combine(AppContext.BaseDirectory, "staticfiles/chirp_cli_db.csv");


app.MapGet("/cheeps", () =>
{

    //READ
    if (!File.Exists(filePath))
    {
        return null;
    }

    var records = new List<Cheep>();
    using (var reader = new StreamReader(filePath))
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
    using (var stream = File.Open(filePath, FileMode.Append))
    using (var writer = new StreamWriter(stream))
    using (var csv = new CsvWriter(writer, config))
    {
        writer.Write("\n");
        csv.WriteRecord(cheep);
    }
});

app.Run();