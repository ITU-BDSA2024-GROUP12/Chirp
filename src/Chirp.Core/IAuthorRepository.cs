namespace Chirp.Core;

public interface IAuthorRepository
{
    public bool CreateAuthor(AuthorDTO author);
    public Task<AuthorDTO> GetAuthorByEmail(string email);
    public Task<AuthorDTO> GetAuthorByName(string name);
    public Task<AuthorDTO> GetAuthor(string name, string email);
    public void AnonymizeUser(string name, string email);
}