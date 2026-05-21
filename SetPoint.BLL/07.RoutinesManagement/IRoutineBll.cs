using SetPoint.BLL._07.RoutinesManagement.Dto;

namespace SetPoint.BLL._07.RoutinesManagement
{
    public interface IRoutineBll
    {
        Task<bool> SyncRoutine(RoutineDto routineDto);
        Task<bool> CreateRoutine(RoutineDto routineDto);
        Task<bool> UpdateRoutine(RoutineDto routineDto);
        Task<RoutineDto?> GetRoutineById(Guid id);
        Task<IEnumerable<RoutineDto>> GetAllPersonalRoutines(Guid userId);
        Task<IEnumerable<RoutineDto>> GetAllRoutines();
        Task<bool> DeleteRoutine(Guid id);
    }
}
