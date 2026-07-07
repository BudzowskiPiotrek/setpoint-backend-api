using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SetPoint.BLL._12.FeedEventManagement.Dto;
using SetPoint.DAL._1.Entity;
using SetPoint.DAL._2.Context;

namespace SetPoint.BLL._12.FeedEventManagement
{
    public class FeedEventBll : IFeedEventBll
    {
        #region Fields
        private readonly IMapper _mapper;
        private readonly SetPointDbContext _context;
        #endregion

        #region Constructors
        public FeedEventBll(SetPointDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        #endregion

        #region Methods
        public async Task<bool> SyncFeedEvent(FeedEventDto dto)
        {
            var existing = await _context.FeedEvents.FirstOrDefaultAsync(fe => fe.Id == dto.Id);

            if (existing == null)
            {
                var entity = _mapper.Map<FeedEvent>(dto);
                await _context.FeedEvents.AddAsync(entity);
            }
            else
            {
                if (existing.DeletedAt != null && dto.DeletedAt == null)
                {
                    existing.DeletedAt = null;
                }

                if (dto.UpdatedAt > existing.UpdatedAt || existing.UpdatedAt == null)
                {
                    _mapper.Map(dto, existing);
                    _context.FeedEvents.Update(existing);
                }
                else
                {
                    return true;
                }
            }
            return await _context.SaveChangesAsync() > 0;
        }
        #endregion
    }
}