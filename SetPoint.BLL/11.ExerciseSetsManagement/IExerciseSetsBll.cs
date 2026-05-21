using SetPoint.BLL._11.ExerciseSetsManagement.Dto;

namespace SetPoint.BLL._11.ExerciseSetsManagement
{
    public interface IExerciseSetsBll
    {
        Task<bool> SyncExerciseSet(ExerciseSetsDto dto);
        Task<bool> CreateExerciseSet(ExerciseSetsDto exerciseSetDto);
        Task<bool> UpdateExerciseSet(ExerciseSetsDto exerciseSetDto);
        Task<ExerciseSetsDto?> GetExerciseSetById(Guid id);
        Task<IEnumerable<ExerciseSetsDto>> GetAllExerciseSetsByWorkoutExerciseId(Guid workoutExerciseId);
        Task<bool> DeleteExerciseSet(Guid id);
    }
}
