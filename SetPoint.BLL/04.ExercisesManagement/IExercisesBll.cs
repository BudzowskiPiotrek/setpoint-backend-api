using SetPoint.BLL._04.ExercisesManagement.Dto;

namespace SetPoint.BLL._04.ExercisesManagement
{
    public interface IExercisesBll
    {
        Task<bool> SyncExercise(ExercisesDto exerciseDto);
        Task<bool> CreateExercise(ExercisesDto exerciseDto);
        Task<bool> UpdateExercise(ExercisesDto exerciseDto);
        Task<ExercisesDto?> GetExerciseById(Guid id);
        Task<IEnumerable<ExercisesDto>> GetAllExercises();
        Task<bool> DeleteExercise(Guid id);
    }
}
