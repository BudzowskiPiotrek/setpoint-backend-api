using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SetPoint.BLL._02.UserRelationManagement.Dto;
using SetPoint.DAL._1.Entity;
using SetPoint.DAL._2.Context;

namespace SetPoint.BLL._02.UserRelationManagement
{
    public class UserRelationBll : IUserRelationBll
    {
        #region Fields
        private readonly IMapper _mapper;
        private readonly SetPointDbContext _context;
        #endregion


        #region Constructors
        public UserRelationBll(SetPointDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        #endregion


        #region Methods
        public async Task<bool> SyncUserRelation(UserRelationDto dto)
        {
            var existing = await _context.UsersRelations
                .FirstOrDefaultAsync(b => b.Id == dto.Id ||
                                         (b.UserId == dto.UserId && b.FriendId == dto.FriendId) ||
                                         (b.UserId == dto.FriendId && b.FriendId == dto.UserId));
            if (existing == null)
            {
                var entity = _mapper.Map<UsersRelations>(dto);
                await _context.UsersRelations.AddAsync(entity);
            }
            else
            {
                if (existing.Status == RelationStatus.Rejected)
                {
                    _mapper.Map(dto, existing);
                    existing.Status = RelationStatus.Pending;
                    existing.UpdatedAt = DateTime.UtcNow;
                    _context.UsersRelations.Update(existing);
                }
                else if (existing.Status == RelationStatus.Pending && dto.Status == RelationStatus.Pending)
                {
                    if (dto.UserId != existing.UserId)
                    {
                        _mapper.Map(dto, existing);
                        existing.Status = RelationStatus.Accepted;
                        existing.UpdatedAt = DateTime.UtcNow;
                        _context.UsersRelations.Update(existing);
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
                    _context.UsersRelations.Update(existing);
                }
                else return true;
            }

            return await _context.SaveChangesAsync() > 0;
        }
        #endregion
    }
}
