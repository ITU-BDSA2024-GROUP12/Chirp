using Microsoft.Data.Sqlite;
using SQLitePCL;
using Microsoft.Extensions.FileProviders; 
using CheepViewModel;
namespace Chirp.SQLite;

public class DBFacade<T> : IDatabaseRepository<T> where T : CheepViewModel.CheepViewModel
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
        conn.Open();
        var command = conn.CreateCommand();
        command.CommandText = @"SELECT username, text, pub_date FROM message m  JOIN user u ON m.author_id = u.user_id;";

        using var reader = command.ExecuteReader();
        List<T> cheeps = new List<T>();
        while (reader.Read())
        {
            var cheep = (T)new CheepViewModel.CheepViewModel(reader.GetString(0), reader.GetString(1), reader.GetString(2));
            cheeps.Add(cheep);
        }
        conn.Close();
        return cheeps;
    }
}