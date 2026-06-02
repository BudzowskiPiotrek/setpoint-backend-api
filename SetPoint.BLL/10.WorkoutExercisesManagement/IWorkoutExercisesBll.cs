using SetPoint.BLL._10.WorkoutExercisesManagement.Dto;

namespace SetPoint.BLL._10.WorkoutExercisesManagement
{
    public interface IWorkoutExercisesBll
    {
        Task<bool> SyncWorkoutExercise(WorkoutExercisesDto dto);
    }
}
