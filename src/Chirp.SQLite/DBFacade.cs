using Microsoft.Data.Sqlite;
using SQLitePCL;
using Microsoft.Extensions.FileProviders; 

namespace Chirp.SQLite;

public class DBFacade<T> : IDatabaseRepository<T>
{
    private SqliteConnection conn;
    private static DBFacade<T> instance;
    
    private DBFacade()
    {
        //Create connection to the SQLite3 DB file
        //Using embbeded ressources
        //string envFilePath = Environment.GetEnvironmentVariable("CHIRPDBPATH");
        
        string envFilePath = Path.GetRelativePath(Directory.GetCurrentDirectory(), @"..\Chirp.SQLite\data\chirp.db");

        // If no environment variable is defined, default to the user's temp directory with chirp.db
        string defaultPath = Path.Combine(Path.GetTempPath(), "chirp.db");
        

        if (envFilePath == null)
        {
            envFilePath = defaultPath;
        }
        Console.WriteLine(envFilePath);
        conn = new SqliteConnection($"Data Source={envFilePath}");
        conn.Open();
    }


    public static DBFacade<T> getInstance()
    {
        if (true/*instance != null*/) //TESTING, CHANGE BACK TO == WHEN DONE
        {
            instance = new DBFacade<T>();
        }
        return instance;
    }

    public void Store(IEnumerable<T> record)
    {
        return;
    }

    public IEnumerable<T> Read(int? limit = null)
    {
        var command = conn.CreateCommand();
        command.CommandText = "SELECT * FROM message";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            Console.WriteLine(reader.GetString(0));
        }
        return null;
    }

    public void closeConnection()
    {
        conn.Close();
    }
}