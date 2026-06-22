using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SetPoint.BLL._0.Infrastructure;
using SetPoint.BLL._02.UserRelationManagement;
using SetPoint.BLL._02.UsersInvitationManagement.Dto;
using SetPoint.BLL._02.UsersManagement;
using SetPoint.BLL._02.UsersManagement.Dto;
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
        private readonly IUserBll _userBll;
        private readonly IUserRelationBll _userRelationBll;
        private readonly SetPointDbContext _context;
        private readonly string _downloadUrl;
        private readonly string _activationUrl;
        #endregion


        #region Constructors
        public UsersInvitationBll(
            IConfiguration config,
            IEmailService emailService,
            IUserBll userBll,
            IUserRelationBll userRelationBll,
            SetPointDbContext context,
            ILogger<UsersInvitationBll> logger)
        {
            _config = config;
            _emailService = emailService;
            _userBll = userBll;
            _userRelationBll = userRelationBll;
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
            var existing = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (existing != null) throw new InvalidOperationException("This email already exist");

            var dateNow = DateTime.UtcNow;

            var newInvitation = new UsersInvitations
            {
                Id = dto.Id,
                CreatedAt = dateNow,
                UpdatedAt = dateNow,
                Email = dto.Email,
                Token = Guid.NewGuid(),
                SenderUserId = dto.SenderUserId,
                ExpiresAt = dateNow.AddDays(2),
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

        public async Task<LoginResponseDto?> AcceptInvitationAsync(Guid token, string fullName, string password)
        {
            var invitation = await _context.UsersInvitations.FirstOrDefaultAsync(u => u.Token == token);
            if (invitation == null || invitation.ExpiresAt < DateTime.UtcNow)
                return null;

            var loginDto = await _userBll.CreateUserAsync(new UserDto
            {
                FullName = fullName,
                Password = password,
                Email = invitation.Email
            });

            if (loginDto?.User?.Id is not Guid newUserId)
                return null;

            invitation.Status = InvitationStatus.Accepted;
            await _context.SaveChangesAsync();

            if (invitation.SenderUserId is Guid senderId)
                await _userRelationBll.CreateFriendshipAsync(senderId, newUserId);

            return loginDto;
        }
        #endregion
    }
}
