using SetPoint.BLL._10.WorkoutExercisesManagement.Dto;

namespace SetPoint.BLL._10.WorkoutExercisesManagement
{
    public interface IWorkoutExercisesBll
    {
        Task<bool> SyncWorkoutExercise(WorkoutExercisesDto dto);
        Task<bool> CreateWorkoutExercise(WorkoutExercisesDto workoutExerciseDto);
        Task<bool> UpdateWorkoutExercise(WorkoutExercisesDto workoutExerciseDto);
        Task<WorkoutExercisesDto?> GetWorkoutExerciseById(Guid id);
        Task<IEnumerable<WorkoutExercisesDto>> GetAllWorkoutExercisesBySessionId(Guid sessionId);
        Task<bool> DeleteWorkoutExercise(Guid id);
    }
}
