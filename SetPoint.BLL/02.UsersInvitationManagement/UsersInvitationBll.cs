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
            var existingInvitation = await _context.UsersInvitations.FirstOrDefaultAsync(u => u.Id == dto.Id);
            if (existingInvitation != null) throw new InvalidOperationException("Duplicate invitation attempt.");

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

            string htmlBody = $@" <div style='font-family:Segoe UI, Arial, sans-serif; max-width:600px; margin:auto; color:#222; line-height:1.7;'> 
                <h2 style='color:#2E8B57;'>⚔️ ¡Una nueva aventura te espera!</h2> 
                <p> Has sido invitado a unirte a la <strong>familia HabityFit</strong>. Todo héroe comienza con una decisión... hoy empieza la tuya. </p>
                <p> 💪 Entrena. Sube de nivel. Rompe tus propios límites. </p>
                <hr style='border:none; border-top:1px solid #ddd; margin:24px 0;' /> 
                <h3>📲 Paso 1: Descarga la aplicación</h3> 
                <p> <a href='{_downloadUrl}' style='color:#2E8B57; font-weight:bold;'> Descargar HabityFit </a> </p> 
                <h3>🛡️ Paso 2: Reclama tu lugar</h3> 
                <p> Cuando tengas la aplicación instalada, pulsa el siguiente enlace para crear tu cuenta y comenzar tu aventura: </p> 
                <p> <a href='{_activationUrl}{newInvitation.Token}' style='background:#2E8B57;color:white;padding:12px 20px;text-decoration:none;border-radius:8px;display:inline-block;'> Crear mi cuenta </a> </p> 
                <p style='margin-top:30px;font-size:13px;color:#777;'> El reino necesita nuevos campeones. ¿Aceptarás la misión? </p> </div>";

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
