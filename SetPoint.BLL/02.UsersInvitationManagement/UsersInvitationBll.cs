using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SetPoint.BLL._0.Infrastructure;
using SetPoint.BLL._02.UsersInvitationManagement.Dto;
using SetPoint.DAL._1.Entity;
using SetPoint.DAL._2.Context;

namespace SetPoint.BLL._02.UsersInvitationManagement
{
    public class UsersInvitationBll : IUsersInvitationBll
    {
        #region Fields
        private readonly ILogger<UsersInvitationBll> _logger;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;
        private readonly SetPointDbContext _context;
        private readonly string _downloadUrl;
        private readonly string _activationUrl;
        #endregion


        #region Constructors
        public UsersInvitationBll(
            IConfiguration config,
            IEmailService emailService,
            SetPointDbContext context,
            ILogger<UsersInvitationBll> logger)
        {
            _config = config;
            _emailService = emailService;
            _context = context;
            _logger = logger;

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

            if (emailResult) newInvitation.Sended = true;
            else _logger.LogWarning("Failed to send invitation email to {Email}.", dto.Email);

            await _context.UsersInvitations.AddAsync(newInvitation);
            await _context.SaveChangesAsync();

            return emailResult;
        }

        public async Task<bool> AcceptInvitationAsync(Guid token, Guid acceptingUserId)
        {
            var invitation = await _context.UsersInvitations.FirstOrDefaultAsync(i => i.Token == token);
            if (invitation == null) return false;

            invitation.Status = InvitationStatus.Accepted;
            invitation.UpdatedAt = DateTime.UtcNow;
            return await _context.SaveChangesAsync() > 0;
        }
        #endregion
    }
}
