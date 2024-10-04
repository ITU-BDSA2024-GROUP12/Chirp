using System.Diagnostics;
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
        string path;
        string envstring = Environment.GetEnvironmentVariable("CHIRPDBPATH");
        if (envstring is not null)
        {
            Console.WriteLine("using CHIRPDBPATH with path: " + Environment.GetEnvironmentVariable("CHIRPDBPATH"));
            conn = new SqliteConnection($"Data Source={envstring}");
        }
        else // If no environment variable is defined, default to the user's temp directory with chirp.db
        {
            Console.WriteLine("no environment variable CHIRPDBPATH, using temp DB path");
            path = Path.Combine(Path.GetTempPath(), "chirp.db");
            
            Console.WriteLine("using temp DB path: " + path);

            string sqlScript =
                """drop table if exists user;create table user (user_id integer primary key autoincrement, username string not null,  email string not null,  pw_hash string not null);drop table if exists message;create table message (  message_id integer primary key autoincrement,  author_id integer not null,  text string not null,  pub_date integer);""";
            conn = new SqliteConnection($"Data Source={path}");
            
            conn.Open();

            // Create a command to execute the SQL script
            using (SqliteCommand command = new SqliteCommand(sqlScript, conn))
            {
                // Execute the script
                command.ExecuteNonQuery();
            }

            conn.Close();
            
        }
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

    public IEnumerable<T> ReadFromAuthor(string author, int? page = null)
    {
        conn.Open();
        var command = conn.CreateCommand();

        if (page.HasValue)
        {
            command.CommandText = "SELECT text, pub_date FROM message m  JOIN user u ON m.author_id = u.user_id WHERE u.username = @author ORDER BY pub_date DESC LIMIT 32 OFFSET @page;";
            command.Parameters.AddWithValue("@page", 32*page.Value);
        }
        else
        {
            command.CommandText = @"SELECT text, pub_date FROM message m  JOIN user u ON m.author_id = u.user_id WHERE u.username = @author ORDER BY pub_date DESC";
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

    

    public IEnumerable<T> Read(int? page = null)
    {
        conn.Open();
        var command = conn.CreateCommand();

        if (page.HasValue)
        {
            command.CommandText = @"SELECT username, text, pub_date FROM message m  JOIN user u ON m.author_id = u.user_id ORDER BY pub_date DESC LIMIT 32 OFFSET @page;";
            command.Parameters.AddWithValue("@page", 32*page.Value);
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