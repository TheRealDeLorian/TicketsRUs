using LibraryTRU.Data;
using LibraryTRU.IServices;
using MauiTRU.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace MauiTRU.Services
{
    public class MauiConcertService : IConcertService
    {
        LocalTRUDatabase _database;                          //Instead of a client that will call the API's
        public MauiConcertService(LocalTRUDatabase database) //We are now injecting our local database 
        {
            _database = database;
        }

        public Task AddNewAsync(Concert concert)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Concert>> GetAll()
        {
            return await _database.GetConcertsAsync();
        }
    }
}
