using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SetPoint.BLL._02.UserRelationManagement.Dto;
using SetPoint.DAL._1.Entity;
using SetPoint.DAL._2.Context;

namespace SetPoint.BLL._02.UserRelationManagement
{
    public class UserRelationBll : IUserRelationBll
    {
        #region Fields

        private readonly IConfiguration _config;

        private readonly IMapper _mapper;

        private readonly string _connectionString;

        #endregion


        #region Constructors

        public UserRelationBll(IConfiguration config)
        {
            _config = config;
            // Retrieve the connection string from configuration
            _connectionString = _config.GetConnectionString("PostgreConnection") ??
                 throw new InvalidOperationException("Connection string 'PostgreConnection' not found.");

            // Configure AutoMapper
            var conMap = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserRelationDto, UsersRelations>();
            });
            _mapper = conMap.CreateMapper();
        }

        #endregion


        #region Methods

        public async Task<bool> SyncUserRelation(UserRelationDto dto)
        {
            using (var context = new SetPointDbContext(_connectionString))
            {
                var existing = await context.UsersRelations
                    .FirstOrDefaultAsync(b => b.Id == dto.Id ||
                                             (b.UserId == dto.UserId && b.FriendId == dto.FriendId) ||
                                             (b.UserId == dto.FriendId && b.FriendId == dto.UserId));
                if (existing == null)
                {
                    var entity = _mapper.Map<UsersRelations>(dto);
                    await context.UsersRelations.AddAsync(entity);
                }
                else
                {
                    if (existing.Status == RelationStatus.Rejected)
                    {
                        _mapper.Map(dto, existing);
                        existing.Status = RelationStatus.Pending;
                        existing.UpdatedAt = DateTime.UtcNow;
                        context.UsersRelations.Update(existing);
                    }
                    else if (existing.Status == RelationStatus.Pending && dto.Status == RelationStatus.Pending)
                    {
                        if (dto.UserId != existing.UserId)
                        {
                            _mapper.Map(dto, existing);
                            existing.Status = RelationStatus.Accepted;
                            existing.UpdatedAt = DateTime.UtcNow;
                            context.UsersRelations.Update(existing);
                        }
                        else return true;
                    }
                    else if (dto.UpdatedAt > existing.UpdatedAt || existing.UpdatedAt == null)
                    {
                        if (existing.DeletedAt != null && dto.DeletedAt == null)
                        {
                            existing.DeletedAt = null;
                        }
                        _mapper.Map(dto, existing);
                        context.UsersRelations.Update(existing);
                    }
                    else return true;
                }

                return await context.SaveChangesAsync() > 0;
            }
        }


        #endregion
    }
}
