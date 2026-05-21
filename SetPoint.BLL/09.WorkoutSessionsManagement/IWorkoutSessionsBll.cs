using SetPoint.BLL._09.WorkoutSessionsManagement.Dto;

namespace SetPoint.BLL._09.WorkoutSessionsManagement
{
    public interface IWorkoutSessionsBll
    {
        Task<bool> SyncWorkoutSession(WorkoutSessionsDto dto);
        Task<bool> CreateSession(WorkoutSessionsDto workoutSessionDto);
        Task<bool> UpdateSession(WorkoutSessionsDto workoutSessionDto);
        Task<WorkoutSessionsDto?> GetSessionById(Guid id);
        Task<IEnumerable<WorkoutSessionsDto>> GetAllPersonalSessions(Guid userId);
        Task<bool> DeleteSession(Guid id);
    }
}
