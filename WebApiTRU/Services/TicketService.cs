using LibraryTRU.Data;
using LibraryTRU.Exceptions;
using Microsoft.EntityFrameworkCore;
namespace WebApiTRU.Services;

public class TicketService : ITicketService
{
    PostgresContext _ticketContext;
    public TicketService(IDbContextFactory<PostgresContext> contxtFact)
    {
        _ticketContext = contxtFact.CreateDbContext();
    }

    public async Task<Ticket> AddTicket(string email, int concertId)
    {
        Ticket ticket = new Ticket()
        {
            Email = email,
            ConcertId = concertId,
            Qrhash = await GenerateTicketHash()
        };

        await _ticketContext.Tickets.AddAsync(ticket);
        await _ticketContext.SaveChangesAsync();
        return ticket;
    }

    public async Task<IEnumerable<Ticket>> GetAll()
    {
        return await _ticketContext.Tickets.Include(tc => tc.Concert).ToListAsync();
    }

    public async Task<IEnumerable<Ticket>> GetAll(string email)
    {
        return await _ticketContext.Tickets.Where(tc => tc.Email == email).ToListAsync();
    }

    public async Task<Ticket> GetTicketById(int id)
    {
        return await _ticketContext.Tickets.Where(hc => hc.Id == id).FirstAsync();
    }

    public async Task DeleteTicket(int id)
    {
        await _ticketContext.Tickets.Where(hc => hc.Id == id).ExecuteDeleteAsync();
    }

    public async Task ScanTicket(string qrHash) 
    {
        try
        {
            Ticket target = await _ticketContext.Tickets.Where(qr => qr.Qrhash == qrHash).SingleAsync();

            if (target.Timescanned is not null)
                throw new TicketAlreadyScannedException();

            target.Timescanned = DateTime.Now;
            _ticketContext.Update(target);
            await _ticketContext.SaveChangesAsync();
        }
        catch (ArgumentNullException e)
        {
            throw new TicketNotFoundException();
        }
    }

    async Task<string> GenerateTicketHash()
    {
        char[] hash = new char[16];

        for (int i = 0; i < 16; i++)
            hash[i] = (char)Random.Shared.Next(33, 127);

        IEnumerable<Ticket> tickets = await GetAll();

        if(tickets.Where(t => t.Qrhash == hash.ToString()).Count() > 0)
            return await GenerateTicketHash();

        return new string(hash);
    }
}
