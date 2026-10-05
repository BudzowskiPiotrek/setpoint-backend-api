using HabityFit.BLL._10.WorkoutExercisesManagement.Dto;

namespace HabityFit.BLL._10.WorkoutExercisesManagement
{
    public interface IWorkoutExercisesBll
    {
        Task<bool> SyncWorkoutExercise(WorkoutExercisesDto dto);
    }
}
