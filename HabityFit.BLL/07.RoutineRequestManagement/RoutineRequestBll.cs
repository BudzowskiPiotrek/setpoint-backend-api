using AutoMapper;
using Microsoft.EntityFrameworkCore;
using HabityFit.BLL._07.RoutineRequestManagement.Dto;
using HabityFit.BLL._07.RoutinesManagement;
using HabityFit.DAL._1.Entity;
using HabityFit.DAL._2.Context;

namespace HabityFit.BLL._07.RoutineRequestManagement
{
    public class RoutineRequestBll : IRoutineRequestBll
    {
        #region Fields
        private readonly IMapper _mapper;
        private readonly HabityFitDbContext _context;
        private readonly IRoutineBll _routine;
        #endregion


        #region Constructors
        public RoutineRequestBll(IRoutineBll routine, HabityFitDbContext context, IMapper mapper)
        {
            _context = context;
            _routine = routine;
            _mapper = mapper;
        }
        #endregion


        #region Methods
        public async Task<bool> SyncRoutineRequest(RoutineRequestDto dto)
        {
            var existing = await _context.RoutineRequests
                .FirstOrDefaultAsync(r => r.Id == dto.Id ||
                                         (r.SenderId == dto.SenderId &&
                                          r.ReceiverId == dto.ReceiverId &&
                                          r.RoutineId == dto.RoutineId));

            if (existing == null)
            {
                var entity = _mapper.Map<RoutineRequests>(dto);
                await _context.RoutineRequests.AddAsync(entity);
            }
            else
            {
                var existingId = existing.Id;
                if (existing.Status == RequestStatus.Rejected)
                {
                    _mapper.Map(dto, existing);
                    existing.Id = existingId;
                    existing.Status = RequestStatus.Pending;
                    existing.UpdatedAt = DateTime.UtcNow;
                    _context.RoutineRequests.Update(existing);
                }
                else if (dto.UpdatedAt > existing.UpdatedAt || existing.UpdatedAt == null)
                {
                    if (existing.DeletedAt != null && dto.DeletedAt == null)
                    {
                        existing.DeletedAt = null;
                    }

                    if (dto.Status == RequestStatus.Accepted)
                    {
                        var copy = await _routine.CloneRoutineForUserAsync(existing.RoutineId, existing.ReceiverId);
                        if (!copy) return false;
                    }
                    _mapper.Map(dto, existing);
                    existing.Id = existingId;
                    _context.RoutineRequests.Update(existing);
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
