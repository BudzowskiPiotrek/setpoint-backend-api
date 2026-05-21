
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
        //################################################# User Authentication

        public Task<LoginResponseDto?> Login(LoginRequestDto loginRequest)
        {
            return _authService.AuthenticateAsync(loginRequest.Email, loginRequest.Password);
        }

        public async Task<bool> Logout(Guid userId)
        {
            using (var context = new SetPointDbContext(_connectionString))
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId && u.DeletedAt == null);

                if (user == null)
                    throw new InvalidOperationException("User not found");

                var log = new Logs
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    UserId = userId,
                    Type = "Logout"

                };
                await context.Logs.AddAsync(log);

                return await context.SaveChangesAsync() > 0;

            }
        }

        //################################################# User Management

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

        public async Task<bool> CreateUser(UserDto userDto)
        {
            if (userDto == null)
                throw new Exception("User data cannot be null");

            using (var context = new SetPointDbContext(_connectionString))
            {
                var exists = await context.Users.AnyAsync(u => u.Email == userDto.Email);

                if (exists)
                    throw new Exception("A user with this email already exists.");

                var userEntity = _mapper.Map<Users>(userDto);

                userEntity.Id = Guid.NewGuid();
                userEntity.PasswordHash = _passwordService.HashPassword(userDto.Password);
                userEntity.CreatedAt = DateTime.UtcNow;

                await context.Users.AddAsync(userEntity);

                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<bool> UpdateUser(UserDto userDto)
        {
            if (userDto == null)
                throw new Exception("User data cannot be null");

            using (var context = new SetPointDbContext(_connectionString))
            {

                var userEntity = await context.Users.FirstOrDefaultAsync(u => u.Id == userDto.Id && u.DeletedAt == null);

                if (userEntity == null)
                    throw new InvalidOperationException("User not found");

                if (!string.IsNullOrWhiteSpace(userDto.Email) && userDto.Email != userEntity.Email)
                {
                    bool emailBusy = await context.Users.AnyAsync(u => u.Email == userDto.Email && u.Id != userEntity.Id);

                    if (emailBusy)
                        throw new InvalidOperationException("Email is already taken by another user.");

                    userEntity.Email = userDto.Email;
                }

                userEntity.FullName = userDto.FullName;
                userEntity.BirthDate = userDto.BirthDate;
                userEntity.Height = userDto.Height;
                userEntity.UpdatedAt = DateTime.UtcNow;

                if (!string.IsNullOrWhiteSpace(userDto.Password))
                {
                    userEntity.PasswordHash = _passwordService.HashPassword(userDto.Password);
                }

                context.Users.Update(userEntity);
                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<bool> DeleteUser(Guid userId)
        {
            using (var context = new SetPointDbContext(_connectionString))
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId && u.DeletedAt == null);

                if (user == null)
                    throw new InvalidOperationException("User not found");

                user.DeletedAt = DateTime.UtcNow;

                context.Users.Update(user);

                return await context.SaveChangesAsync() > 0;
            }
        }

        //################################################# User Retrieval
        public async Task<UserReadDto?> GetUserByEmail(string email)
        {
            using (var context = new SetPointDbContext(_connectionString))
            {

                var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email && u.DeletedAt == null);

                return user == null ? null : _mapper.Map<UserReadDto>(user);

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

        public async Task<IEnumerable<UserReadDto>> GetAllUsers()
        {
            using (var context = new SetPointDbContext(_connectionString))
            {
                var users = await context.Users.Where(u => u.DeletedAt == null && u.DeletedAt == null).ToListAsync();

                return _mapper.Map<IEnumerable<UserReadDto>>(users);
            }
        }



        #endregion
    }
}
