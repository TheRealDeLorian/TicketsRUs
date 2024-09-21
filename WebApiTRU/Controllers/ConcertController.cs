using LibraryTRU.Data;

namespace WebApiTRU.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class ConcertController : Controller
{
    IConcertService _cs;
    public ConcertController(IConcertService concertService)
    {
        _cs = concertService;
    }

    [HttpGet("getall")]
    public async Task<IEnumerable<Concert>> GetAllConcerts()
    {
        return await _cs.GetAll();
    }
}
