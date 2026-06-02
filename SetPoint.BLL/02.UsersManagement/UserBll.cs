
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SetPoint.BLL._02.UsersManagement.Dto;
using SetPoint.BLL._1.Security;
using SetPoint.DAL._1.Entity;
using SetPoint.DAL._2.Context;

namespace SetPoint.BLL._02.UsersManagement
{
    public class UserBll : IUserBll
    {
        #region Fields

        private readonly IConfiguration _config;

        private readonly IPasswordService _passwordService;

        private readonly IAuthService _authService;

        private readonly IMapper _mapper;

        private readonly string _connectionString;

        #endregion


        #region Constructors
        public UserBll(IConfiguration config, IAuthService authService, IPasswordService passwordService)
        {
            _config = config;

            _passwordService = passwordService;

            _authService = authService;


            var conMap = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Users, UserReadDto>();
                cfg.CreateMap<UserDto, Users>();
            });

            _mapper = conMap.CreateMapper();

            _connectionString = _config.GetConnectionString("PostgreConnection")
                ?? throw new InvalidOperationException("Connection string 'PostgreConnection' not found.");
        }


        #endregion


        #region Methods
        public Task<LoginResponseDto?> Login(LoginRequestDto loginRequest)
        {
            return _authService.AuthenticateAsync(loginRequest.Email, loginRequest.Password);
        }

        public async Task<bool> SyncUser(UserReadDto dto)
        {
            using (var context = new SetPointDbContext(_connectionString))
            {
                var existing = await context.Users.FirstOrDefaultAsync(u => u.Id == dto.Id);
                if (existing == null)
                {
                    return false;
                }

                if (existing.DeletedAt != null && dto.DeletedAt == null)
                {
                    existing.DeletedAt = null;
                }

                if (dto.UpdatedAt > (existing.UpdatedAt ?? DateTime.MinValue))
                {
                    existing.BirthDate = dto.BirthDate;
                    existing.FullName = dto.FullName;
                    existing.Email = dto.Email;
                    existing.Sex = dto.Sex;
                    existing.Height = dto.Height;
                    existing.UpdatedAt = DateTime.UtcNow;
                    return await context.SaveChangesAsync() > 0;
                }
                return true;
            }
        }

        public async Task<UserReadDto?> GetUserById(Guid userId)
        {
            using (var context = new SetPointDbContext(_connectionString))
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId && u.DeletedAt == null);

                return user == null ? null : _mapper.Map<UserReadDto>(user);
            }
        }
        #endregion
    }
}
