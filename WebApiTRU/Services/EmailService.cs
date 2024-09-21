using System.Text;
using LibraryTRU.Data.DTOs;
using MimeKit;
using MimeKit.Utils;
using Net.Codecrete.QrCodeGenerator;
using System.IO;
using Aspose.Html.Converters;
using Aspose.Html.Saving;
namespace WebApiTRU.Email;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    public EmailService(IConfiguration config)
    {
        _config = config;
    }
    public void SendEmail(EmailInfoDTO emailInfo)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("TRU", "ticketsrus4@gmail.com"));
        message.To.Add(new MailboxAddress("", emailInfo.Email));
        message.Subject = emailInfo.Subject;
        var builder = new BodyBuilder();
        string qrString = MakeCode(emailInfo.QrHash);
        builder.HtmlBody = "<html>" +
                "<body>" +
                $"<p>{emailInfo.Message}</p>" +
                $"<img alt=\"{qrString}\" src=\"ReplaceThis\" width=\"50%\" height=\"auto\"/>" +
                "</body>" +
                "</html>";

        var qrCodeFilePath = Path.Combine(Directory.GetCurrentDirectory(), "qrCode");

        Converter.ConvertHTML($"<img alt=\"{qrString}\" src=\"{qrString}\" width=\"auto\" />", ".", new ImageSaveOptions(), qrCodeFilePath);
        using FileStream tempReader = File.OpenRead(qrCodeFilePath);

        var attachment = new MimePart("image", "png")
        {
            Content = new MimeContent(tempReader),
            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
            ContentTransferEncoding = ContentEncoding.Base64,
            FileName = Path.GetFileName(qrCodeFilePath)
        };

        attachment.ContentId = MimeUtils.GenerateMessageId();
        builder.Attachments.Add(attachment);

        builder.HtmlBody = builder.HtmlBody.Replace("ReplaceThis", $"cid:{attachment.ContentId}");

        builder.TextBody = emailInfo.Message;
        message.Body = builder.ToMessageBody();
        using (var client = new MailKit.Net.Smtp.SmtpClient()){
            client.Connect("smtp.gmail.com", 587, false);
            client.Authenticate("ticketsrus4@gmail.com", $"{_config["emailpassword"]}");
            client.Send(message);
            client.Disconnect(true);
        }

        tempReader.Close();
        File.Delete(qrCodeFilePath);
    }

    string MakeCode(string codeValue)
    {
        var qr = QrCode.EncodeText(codeValue, QrCode.Ecc.High);
        return $"data:image/svg+xml;base64,{Convert.ToBase64String(Encoding.UTF8.GetBytes(qr.ToSvgString(4)))}";
    }
}