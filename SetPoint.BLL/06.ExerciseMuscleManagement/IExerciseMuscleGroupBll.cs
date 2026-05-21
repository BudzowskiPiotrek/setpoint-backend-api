using SetPoint.BLL._06.ExerciseMuscleManagement.Dto;

namespace SetPoint.BLL._06.ExerciseMuscleManagement
{
    public interface IExerciseMuscleGroupBll
    {
        Task<bool> SyncExerciseMuscleGroup(ExerciseMuscleDto dto);
        Task<bool> CreateExerciseMuscleGroup(ExerciseMuscleDto exerciseMuscleDto);
        Task<bool> UpdateExerciseMuscleGroup(ExerciseMuscleDto exerciseMuscleDto);
        Task<ExerciseMuscleDto?> GetExerciseMuscleGroupById(Guid id);
        Task<IEnumerable<ExerciseMuscleDto>> GetAllExerciseMuscleGroups(Guid exercise);
        Task<bool> DeleteExerciseMuscleGroup(Guid id);
    }
}
