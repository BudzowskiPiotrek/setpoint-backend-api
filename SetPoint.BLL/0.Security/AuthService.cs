using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SetPoint.BLL._02.UsersManagement;
using SetPoint.BLL._02.UsersManagement.Dto;
using SetPoint.DAL._1.Entity;
using SetPoint.DAL._2.Context;

namespace SetPoint.BLL._1.Security
{
    public class AuthService : IAuthService
    {
        #region Fields

        private readonly IPasswordService _passwordService;

        private readonly ITokenService _tokenService;

        private readonly IConfiguration _config;

        private readonly IMapper _mapper;

        private readonly string _connectionString;

        #endregion


        #region Constructors

        public AuthService(IPasswordService passwordService, ITokenService tokenService, IConfiguration config)
        {
            _passwordService = passwordService;
            _tokenService = tokenService;
            _config = config;
            _connectionString = _config.GetConnectionString("PostgreConnection") ??
                throw new InvalidOperationException("Connection string 'PostgreConnection' not found.");

            var conMap = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Users, UserReadDto>();
                cfg.CreateMap<UserDto, Users>();
            });

            _mapper = conMap.CreateMapper();
        }

        #endregion


        #region Methods

        public async Task<LoginResponseDto?> AuthenticateAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Email and password must be provided.");
            var normalizedEmail = email.Trim().ToLower();
            // Verify user credentials
            using (var context = new SetPointDbContext(_connectionString))
            {
                var log = new Logs
                {
                    Id = Guid.NewGuid(),
                    UserId = null,
                    CreatedAt = DateTime.UtcNow,
                };

                var userEntity = await context.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail && u.DeletedAt == null);

                if (userEntity != null)
                {
                    var lastMinute = DateTime.UtcNow.AddMinutes(-1);
                    var failedAttempts = await context.Logs.CountAsync(l => l.UserId == userEntity.Id &&
                                                                            l.Type == "[INFO] [AUTH] Access Failed - Invalid Password" &&
                                                                            l.CreatedAt > lastMinute);

                    if (failedAttempts >= 3)
                    {
                        log.UserId = userEntity.Id;
                        log.Type = "[WARN] [AUTH] Login Blocked - Too many attempts";
                        await context.Logs.AddAsync(log);
                        await context.SaveChangesAsync();
                        throw new UnauthorizedAccessException("Too many atempts. Account locked for 1 min.");
                    }
                }
                else
                {
                    log.Type = "[INFO] [AUTH] Access Failed - User Not Found";
                    await context.Logs.AddAsync(log);
                    await context.SaveChangesAsync();
                    throw new UnauthorizedAccessException("User not found");

                }
                log.UserId = userEntity.Id;
                // Verify password
                bool isValid = _passwordService.VerifyPassword(userEntity.PasswordHash, password, out bool needRehash);

                if (!isValid)
                {
                    log.Type = "[INFO] [AUTH] Access Failed - Invalid Password";
                    await context.Logs.AddAsync(log);
                    await context.SaveChangesAsync();
                    throw new UnauthorizedAccessException("Invalid password");
                }

                if (needRehash)
                {
                    userEntity.PasswordHash = _passwordService.HashPassword(password);
                    context.Users.Update(userEntity);
                    await context.SaveChangesAsync();
                }
                // Map user entity to DTO
                var userReadDto = _mapper.Map<UserReadDto>(userEntity);

                // Generate JWT token
                var token = _tokenService.CreateToken(userReadDto);

                // Log the login event
                log.UserId = userEntity.Id;
                log.Type = "[INFO] [AUTH] Access Success";
                await context.Logs.AddAsync(log);
                await context.SaveChangesAsync();

                var result = new LoginResponseDto
                {
                    Token = token,
                    User = userReadDto,
                    Expiration = DateTime.UtcNow.AddDays(7)
                };

                return result;
            }
        }

        #endregion
    }
}
