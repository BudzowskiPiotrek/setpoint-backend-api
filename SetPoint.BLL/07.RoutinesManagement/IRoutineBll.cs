using SetPoint.BLL._07.RoutinesManagement.Dto;

namespace SetPoint.BLL._07.RoutinesManagement
{
    public interface IRoutineBll
    {
        Task<bool> SyncRoutine(RoutineDto routineDto);
    }
}
