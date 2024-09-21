using LibraryTRU.Data;

namespace LibraryTRU.IServices;

public interface ITicketService
{
    public Task<IEnumerable<Ticket>> GetAll();
    public Task<Ticket> AddTicket(string email, int concertId);
    public Task ScanTicket(string qrHash);
    public Task DeleteTicket(int id);
    public Task<Ticket> GetTicketById(int id);
}
