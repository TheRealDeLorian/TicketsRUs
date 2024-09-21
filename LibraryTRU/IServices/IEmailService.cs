

using LibraryTRU.Data.DTOs;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace LibraryTRU.IServices
{
    public interface IEmailService
    {
        void SendEmail(EmailInfoDTO emailInfo);
    }
}
