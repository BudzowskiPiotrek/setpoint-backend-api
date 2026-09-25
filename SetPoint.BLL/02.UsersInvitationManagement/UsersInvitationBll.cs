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
        private readonly string _htmlBodyInvitation;
        private readonly string _htmlBodyAccept;
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

            _htmlBodyInvitation = _config["HTML:Invitation"]
                ?? throw new InvalidOperationException("HTML Body of Invitation not found in configuration.");

            _htmlBodyAccept = _config["HTML:Accept"]
                ?? throw new InvalidOperationException("HTML Body of Accept not found in configuration.");
        }
        #endregion


        #region Methods
        public async Task<bool> CreateAndSendValidateAsync(string email)
        {
            var existing = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
            if (existing != null) throw new InvalidOperationException("This email already exist");

            var dateNow = DateTime.UtcNow;

            var newInvitation = new UsersInvitations
            {
                Id = Guid.NewGuid(),
                CreatedAt = dateNow,
                UpdatedAt = dateNow,
                Email = email,
                Token = Guid.NewGuid(),
                SenderUserId = null,
                ExpiresAt = dateNow.AddDays(2),
                Status = InvitationStatus.Pending,
                Sended = false,
            };

            string htmlBody = string.Format(_htmlBodyAccept, newInvitation.Token);

            bool emailResult = await _emailService.SendEmailAsync(email, "HabityFit: El destino ha pronunciado tu nombre. ¿Aceptarás la misión?", htmlBody);


            if (emailResult) newInvitation.Sended = true;
            else _logger.LogWarning("Failed to send accept email to {Email}.", email);

            await _context.UsersInvitations.AddAsync(newInvitation);
            await _context.SaveChangesAsync();

            return emailResult;
        }

        public async Task<bool> CreateAndSendInvitationAsync(UsersInvitationDto dto)
        {
            var existingInvitation = await _context.UsersInvitations.AsNoTracking().FirstOrDefaultAsync(u => u.Id == dto.Id);
            if (existingInvitation != null) throw new InvalidOperationException("Duplicate invitation attempt.");

            var existing = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == dto.Email);
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

            string htmlBody = string.Format(_htmlBodyInvitation, newInvitation.Token);

            bool emailResult = await _emailService.SendEmailAsync(dto.Email, "HabityFit: El destino ha pronunciado tu nombre. ¿Aceptarás la misión?", htmlBody);

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
