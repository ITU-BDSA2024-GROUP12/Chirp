namespace Chirp.SQLite;

public interface IDatabaseRepository<T>
{
    public IEnumerable<T> Read(int? limit = null);
    public void Store(IEnumerable<T> record);
    public IEnumerable<T> ReadFromAuthor(string author, int? limit = null);
}

