using SetPoint.BLL._09.WorkoutSessionsManagement.Dto;

namespace SetPoint.BLL._09.WorkoutSessionsManagement
{
    public interface IWorkoutSessionsBll
    {
        Task<bool> SyncWorkoutSession(WorkoutSessionsDto dto);
    }
}
