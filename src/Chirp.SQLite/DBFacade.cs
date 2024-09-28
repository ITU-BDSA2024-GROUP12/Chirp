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
        if (instance == null) //TESTING, CHANGE BACK TO == WHEN DONE
        {
            instance = new DBFacade<T>();
        }
        return instance;
    }

    public void Store(IEnumerable<T> record)
    {
        return;
    }

    public IEnumerable<T> ReadFromAuthor(string author, int? limit = null)
    {
        conn.Open();
        var command = conn.CreateCommand();

        if (limit.HasValue)
        {
            command.CommandText = @"SELECT text, pub_date FROM message m  JOIN user u ON m.author_id = u.user_id WHERE u.username = @author ORDER BY pub_date LIMIT @limit;";
            command.Parameters.AddWithValue("@limit", limit.Value);
        }
        else
        {
            command.CommandText = @"SELECT text, pub_date FROM message m  JOIN user u ON m.author_id = u.user_id WHERE u.username = @author ORDER BY pub_date";
        }
        command.Parameters.AddWithValue("@author", author);
        
        using var reader = command.ExecuteReader();
        List<T> cheeps = new List<T>();
        while (reader.Read())
        {
            string text = reader.GetString(0);
            string date = UnixTimeStampToDateTimeString(reader.GetInt64(1));
            var cheep = (T)new CheepViewModel.CheepViewModel(author, text, date);
            cheeps.Add(cheep);
        }
        conn.Close();
        return cheeps;
    }

    

    public IEnumerable<T> Read(int? limit = null)
    {
        conn.Open();
        var command = conn.CreateCommand();

        if (limit.HasValue)
        {
            command.CommandText = @"SELECT username, text, pub_date FROM message m  JOIN user u ON m.author_id = u.user_id ORDER BY pub_date DESC LIMIT @limit;";
            command.Parameters.AddWithValue("@limit", limit.Value);
        }
        else
        {
            command.CommandText = @"SELECT username, text, pub_date FROM message m  JOIN user u ON m.author_id = u.user_id ORDER BY pub_date DESC;";
        }
        
        

        using var reader = command.ExecuteReader();
        List<T> cheeps = new List<T>();
        while (reader.Read())
        {
            string author = reader.GetString(0);
            string text = reader.GetString(1);
            string date = UnixTimeStampToDateTimeString(reader.GetInt64(2));
            var cheep = (T)new CheepViewModel.CheepViewModel(author, text, date);
            cheeps.Add(cheep);
        }
        conn.Close();
        return cheeps;
    }
    
    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }
}