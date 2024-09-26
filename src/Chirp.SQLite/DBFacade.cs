using System.Reflection;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.FileProviders;

namespace Chirp.SQLite;

public class DBFacade<T> : IDatabaseRepository<T>
{
    private SqliteConnection conn;
    
    public DBFacade()
    {
        //Create connection to the SQLite3 DB file
        //Using embbeded ressources
        var embeddedProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());
        string sqlDBFilePath = embeddedProvider.GetFileInfo("./data/chirp.db").PhysicalPath;
        //TODO: If no chirp.db is set via environment variables, then default to ./tmp
        conn = new SqliteConnection($"Data Source={sqlDBFilePath}"))

        conn.Open();
    }

    public IEnumerable<T> read()
    {
        var command = conn.CreateCommand();
        command.CommandText = sqlQuery;

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
        }
    }

    public void closeConnection()
    {
        conn.Close();
    }
}