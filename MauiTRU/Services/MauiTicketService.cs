using LibraryTRU.IServices;
using System.Net.Http.Json;
using LibraryTRU.Data;
using MauiTRU.Database;

namespace MauiTRU.Services
{
    public class MauiTicketService(LocalTRUDatabase _context) : ITicketService
    {
        public Task DeleteTicket(int id)
        {
            throw new Exception("Cannot delete a ticket on the mobile app");
        }

        public async Task<IEnumerable<Ticket>> GetAll()
        {
            return await _context.GetTicketsAsync();
        }

        public async Task<Ticket> GetTicketById(int id)
        {
            var t = await _context.GetTicketsAsync();
            return t.Where(t => t.Id == id).FirstOrDefault();
        }

        public async Task ScanTicket(string qrHash)
        {
            await _context.ScanTicketAsync(qrHash);
        }

        Task<Ticket> ITicketService.AddTicket(string email, int concertId)
        {
            throw new Exception("Cannot create a ticket on the mobile app");
        }
    }
}
