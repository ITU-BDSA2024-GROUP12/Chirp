namespace Chirp.SQLite;

public class SQLiteDatabase<T> : IDatabaseRepository<T>
{
    
    
    private SqliteConnnection conn;
    
    public SQLiteDatabase ()
    {
        //Create connection to the SQLite3 DB file
        //Using embbeded ressources
        EmbeddedFileProvider embeddedProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());
        string sqlDBFilePath = embeddedProvider.GetFileInfo("./data/chirp.db").PhysicalPath;
        //TODO: If no chirp.db is set via environment variables, then default to ./tmp
        conn = new SqliteConnection($"Data Source={sqlDBFilePath}"))

        conn.open();
    }

    public IEnumerable<T> read()
    {
        var command = connection.CreateCommand();
        command.CommandText = sqlQuery;

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            Co
        }
    }

    public closeConnection()
    {
        conn.close();
    }

            
            

}