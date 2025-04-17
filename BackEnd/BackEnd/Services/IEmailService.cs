namespace BackEnd.Services
{
    public interface IEmailService
    {
        Task SendAsync(string fromEmail, string fromName,
                       string toEmail, string subject, string body);
    }
}
