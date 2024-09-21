using System.ComponentModel.DataAnnotations;


namespace LibraryTRU.Data.DTOs;

public class EmailInfoDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Subject { get; set; }

    [Required]
    public string Message { get; set; }

    [Required]
    public string QrHash { get; set; }
}