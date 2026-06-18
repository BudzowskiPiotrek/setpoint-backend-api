
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SetPoint.BLL._02.UsersManagement.Dto;
using SetPoint.BLL._1.Security;
using SetPoint.DAL._2.Context;

namespace SetPoint.BLL._02.UsersManagement
{
    public class UserBll : IUserBll
    {
        #region Fields
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly SetPointDbContext _context;
        #endregion


        #region Constructors
        public UserBll(IAuthService authService, SetPointDbContext context, IMapper mapper)
        {
            _context = context;
            _authService = authService;
            _mapper = mapper;
        }
        #endregion


        #region Methods
        public Task<LoginResponseDto?> Login(LoginRequestDto loginRequest)
        {
            return _authService.AuthenticateAsync(loginRequest.Email, loginRequest.Password);
        }

        public async Task<bool> SyncUser(UserReadDto dto)
        {
            var existing = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.Id);
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
                return await _context.SaveChangesAsync() > 0;
            }
            return true;
        }

        public async Task<UserReadDto?> GetUserById(Guid userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId && u.DeletedAt == null);

            return user == null ? null : _mapper.Map<UserReadDto>(user);
        }
        #endregion
    }
}
