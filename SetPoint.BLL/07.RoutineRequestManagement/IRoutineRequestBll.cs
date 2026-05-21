using SetPoint.BLL._07.RoutineRequestManagement.Dto;

namespace SetPoint.BLL._07.RoutineRequestManagement
{
    public interface IRoutineRequestBll
    {
        Task<bool> SyncRoutineRequest(RoutineRequestDto dto);
    }
}
