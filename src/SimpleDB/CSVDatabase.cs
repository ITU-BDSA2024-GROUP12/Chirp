using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace SimpleDB;

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    private string path;
    private static CSVDatabase<T> instance; 
    
    private CSVDatabase(string filename)
    {
        path = filename;
    }

    public static CSVDatabase<T> GetInstance(string filename)
    {
        if (instance == null)
        {
            instance = new CSVDatabase<T>(filename);
        }

        return instance;
    }
    
    public IEnumerable<T> Read (int? limit = null)
    {
        //CSV code by Christian Klingenberg Molusson <cmol@itu.dk>
        var records = new List<T>();
        using (var reader = new StreamReader(path))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            records = csv.GetRecords<T>().ToList();

            return records;
        }
    }
    
    public void Store(IEnumerable<T> records){
        
        //CSV code by Christian Klingenberg Molusson <cmol@itu.dk>
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
    
}