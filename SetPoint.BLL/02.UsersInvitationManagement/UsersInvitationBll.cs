using Microsoft.Extensions.Configuration;
using SetPoint.BLL._0.Security;
using SetPoint.BLL._02.UsersInvitationManagement.Dto;
using SetPoint.DAL._1.Entity;
using SetPoint.DAL._2.Context;

namespace SetPoint.BLL._02.UsersInvitationManagement
{
    public class UsersInvitationBll : IUsersInvitationBll
    {
        #region Fields
        private readonly string _connectionString;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;
        private readonly string _downloadUrl;
        private readonly string _activationUrl;
        #endregion


        #region Constructors
        public UsersInvitationBll(IConfiguration config, IEmailService emailService)
        {
            _config = config;
            _emailService = emailService;
            // Retrieve the connection string from configuration
            _connectionString = _config.GetConnectionString("PostgreConnection") ??
                 throw new InvalidOperationException("Connection string 'PostgreConnection' not found.");

            _downloadUrl = _config["EmailSettings:DownloadUrl"]
                ?? throw new InvalidOperationException("Email DownloadUrl not found in configuration.");

            _activationUrl = _config["EmailSettings:ActivationUrl"]
                ?? throw new InvalidOperationException("Email ActivationUrl not found in configuration.");
        }
        #endregion


        #region Methods
        public async Task<bool> CreateAndSendInvitationAsync(UsersInvitationDto dto)
        {
            var dateNow = DateTime.UtcNow;

            var newInvitation = new UsersInvitations
            {
                Id = dto.Id,
                CreatedAt = dateNow,
                UpdatedAt = dateNow,
                Email = dto.Email,
                Token = Guid.NewGuid(),
                SenderUserId = dto.SenderUserId,
                ExpiresAt = dateNow.AddDays(7),
                Status = InvitationStatus.Pending,
                Sended = false,
            };

            string htmlBody = $"<p>You have been invited to join SetPoint by {_downloadUrl}.</p>";

            bool emailResult = await _emailService.SendEmailAsync(dto.Email, "SetPoint Invitation", htmlBody);

            if (emailResult)
            {
                newInvitation.Sended = true;
            }

            using (var context = new SetPointDbContext(_connectionString))
            {
                await context.UsersInvitations.AddAsync(newInvitation);
                await context.SaveChangesAsync();
            }

            return emailResult;
        }
        #endregion
    }
}
