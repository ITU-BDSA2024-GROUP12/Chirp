﻿namespace Chirp.Core;

public interface ICheepRepository
{
    public Task<List<CheepDTO>> ReadMessage(int page);
    public void UpdateMessage();
    public Task<List<CheepDTO>> ReadMessagesFromAuthor(string author, int page);
    public bool CreateAuthor(AuthorDTO author);
    public bool CreateCheep(AuthorDTO author, string text, string time);
    public Task<AuthorDTO> GetAuthorByEmail(string email);
    public Task<AuthorDTO> GetAuthorByName(string name);
    public Task<AuthorDTO> GetAuthor(string name, string email);
}