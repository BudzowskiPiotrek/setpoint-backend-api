namespace SetPoint.BLL._0.Infrastructure
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody);
    }
}
