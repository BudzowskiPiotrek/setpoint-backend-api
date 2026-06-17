namespace SetPoint.BLL._0.Security
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody);
    }
}
