using LibraryTRU.Data.DTOs;

namespace WebApiTRU.Controllers;

[ApiController]
[Route("/api/email")]
public class EmailController : Controller
{
    private readonly IEmailService emailService;

    public EmailController(IEmailService emailService)
    {
        this.emailService = emailService;
    }

    [HttpPost]
    public IActionResult SendEmail([FromBody] EmailInfoDTO model)
    {
        if (model == null || !ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        emailService.SendEmail(model);

        return Ok();
    }
}