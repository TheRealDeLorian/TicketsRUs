﻿using LibraryTRU.Data;
using Microsoft.EntityFrameworkCore;

namespace WebApiTRU.Services;

public class ConcertService : IConcertService
{
    private readonly PostgresContext _concertcontext;
    public ConcertService(IDbContextFactory<PostgresContext> contxtFact)
    {
        _concertcontext = contxtFact.CreateDbContext();
    }

    public async Task<IEnumerable<Concert>> GetAll()
    {
        return await _concertcontext.Concerts.ToListAsync();
    }

    public async Task AddNewAsync(Concert concert)
    {
       await _concertcontext.Concerts.AddAsync(concert);
        await _concertcontext.SaveChangesAsync();
    }
}
