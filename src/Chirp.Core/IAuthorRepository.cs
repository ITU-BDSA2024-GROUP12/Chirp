namespace Chirp.Core;

public interface IAuthorRepository
{
    public bool CreateAuthor(AuthorDTO author);
    public Task<AuthorDTO> GetAuthorByEmail(string email);
    public Task<AuthorDTO> GetAuthorById(int id);

    public Task<AuthorDTO> GetAuthorByName(string name);
    public Task<AuthorDTO> GetAuthor(string name, string email);
    public Task<List<AuthorDTO>> GetValidUsernames(List<string> mentions);
    public void DeleteUser(string name, string email);
    public bool FollowUser(int authorId, string userName);
    public bool UnfollowUser(int authorId, string userName);
    public Task<List<int>> GetFollowerIds(string userName);
}