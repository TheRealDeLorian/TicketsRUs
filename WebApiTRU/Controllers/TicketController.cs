using LibraryTRU.Data.DTOs;
using LibraryTRU.Exceptions;

namespace WebApiTRU.Controllers;

[ApiController]
[Route("/api/[Controller]")]
public class TicketController : Controller
{
    ITicketService _ts;
    public TicketController(ITicketService ticketService)
    {
        _ts = ticketService;
    }


    [HttpGet("{id}")]
    public async Task<Ticket> GetTicketById(int id)
    {
        var ticket = await _ts.GetTicketById(id);

        if (ticket == null)
        {
            Response.StatusCode = 410;
        }

        return ticket;
    }

    [HttpGet("getall")]
    public async Task<IEnumerable<Ticket>> GetAll()
    {
        return await _ts.GetAll();
    }

    [HttpPost("new")]
    public async Task<ActionResult<Ticket>> PostTicket([FromBody] TicketDTO ticketDTO)
    {
        var ticket = await _ts.AddTicket(ticketDTO.Email, ticketDTO.ConcertId);

        if (ticket == null)
        {
            return BadRequest(); // Or any appropriate status code indicating failure
        }

        return CreatedAtAction(nameof(GetTicketById), new { id = ticket.Id }, ticket);
    }

    [HttpPut("scan")]
    public async Task ScanTicket([FromBody]string qrHash)
    {
        try
        {
            await _ts.ScanTicket(qrHash);
        }
        catch (TicketNotFoundException)
        {
            Response.StatusCode = 410;
        }
        catch (TicketAlreadyScannedException)
        {
            Response.StatusCode = 411;
        }
        catch
        {
            Response.StatusCode = 500;
        }
    }

    [HttpDelete("{id}")]
    public async Task DeleteTicket([FromBody]int id)
    {
        await _ts.DeleteTicket(id);
    }

}
