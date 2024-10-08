﻿namespace Chirp.Core;

public interface ICheepRepository
{
    public void CreateMessage();
    public Task<List<CheepDTO>> ReadMessage(int page);
    public void UpdateMessage();
    public Task<List<CheepDTO>> ReadMessagesFromAuthor(string author, int page);
}