using SQLite;
using LibraryTRU.Data;
using System.Net.Http.Json;
using SQLiteNetExtensionsAsync.Extensions;
using LibraryTRU.Exceptions;

namespace MauiTRU.Database;

public class LocalTRUDatabase
{
    SQLiteAsyncConnection Database;
    IDbPath _ifs;
    HttpClient _client;
    public LocalTRUDatabase(IDbPath ifs, HttpClient client)
    {
        _ifs = ifs;
        _client = client;
    }

    async Task Init()
    {
        if (Database is not null)
            return;

        Database = new SQLiteAsyncConnection(Path.Combine(_ifs.Directory, Constants.DatabaseFilename), Constants.Flags);
        await Database.CreateTableAsync<Concert>();
        await Database.CreateTableAsync<Ticket>();
    }
    public async Task DeleteDatabase()
    {
        await Database.DeleteAllAsync<Ticket>();
        await Database.DeleteAllAsync<Concert>();
    }
    public async Task<List<Ticket>> GetTicketsAsync()
    {
        await Init();
        return await Database.GetAllWithChildrenAsync<Ticket>();
    }
    public async Task<List<Concert>> GetConcertsAsync()
    {
        await Init();
        return await Database.Table<Concert>().ToListAsync();
    }
    public async Task ScanTicketAsync(string qrHash)
    {
        await Init();
        var ticket = await Database.Table<Ticket>().Where(t => t.Qrhash == qrHash).FirstOrDefaultAsync();

        if (ticket is null)
            throw new TicketNotFoundException();

        if (ticket.Timescanned is not null)
            throw new TicketAlreadyScannedException();

        ticket.Timescanned = DateTime.Now;
        await Database.UpdateAsync(ticket);
    }

    public async Task UpdateLocalDbFromMainDb()
    {
        await Init();
        await UpdateLocalTicketsFromMain();
        await UpdateLocalConcertsFromMain();
    }

    public async Task UpdateMainDbFromLocalDb()
    {
        await Init();
        await UpdateMainTicketsFromLocal();
    }

    private async Task UpdateMainTicketsFromLocal()
    {
        var localTickets = await Database.Table<Ticket>().ToListAsync();
        var mainTickets = await _client.GetFromJsonAsync<IEnumerable<Ticket>>("api/ticket/getall");

        foreach (Ticket localTicket in localTickets)
            if(localTicket.Timescanned is not null) //If the local ticket is scanned
                if (mainTickets.Where(mt => mt.Id == localTicket.Id).Single().Timescanned is null) // And the main ticket is not scanned
                    await _client.PutAsJsonAsync("api/ticket/scan", localTicket.Qrhash); // scan the main one
    }

    private async Task UpdateLocalTicketsFromMain()
    {
        var mainTickets = await _client.GetFromJsonAsync<IEnumerable<Ticket>>("api/ticket/getall");
        var localTickets = await Database.Table<Ticket>().ToListAsync();

        foreach (Ticket mainTicket in mainTickets)
        {
            try
            {
                var result = await Database.GetAsync<Ticket>(localTicket => localTicket.Id == mainTicket.Id);

                if (result.Timescanned is not null) // If it has been scanned, update it
                    await Database.UpdateAsync(result);
            }
            catch (InvalidOperationException ex) // Main Ticket not found in local db
            {
                await Database.InsertAsync(mainTicket);
            }
        }

        foreach (Ticket localTicket in localTickets)
            if (mainTickets.Where(mt => mt.Id == localTicket.Id).Count() < 1) // Delete any tickets that are deleted in the main db
                await Database.DeleteAsync(localTicket);
    }

    private async Task UpdateLocalConcertsFromMain()
    {
        var mainConcerts = await _client.GetFromJsonAsync<IEnumerable<Concert>>("api/concert/getall");
        var localConcerts = await Database.Table<Concert>().ToListAsync();

        foreach (var mainConcert in mainConcerts)
        {
            try
            {
                var result = await Database.GetAsync<Concert>(lc => lc.Id == mainConcert.Id);

                if(result != mainConcert) // Not completely sure if this != will work for description and stuff...
                    await Database.UpdateAsync(mainConcert);
            }
            catch (InvalidOperationException ex) // Main Concert not found in local db
            {
                await Database.InsertAsync(mainConcert);
            }
        }

        foreach (var localConcert in localConcerts)
            if (mainConcerts.Where(mt => mt.Id == localConcert.Id).Count() < 1) // Delete any concerts that are deleted in the main db
                await Database.DeleteAsync(localConcert);
    }
}
