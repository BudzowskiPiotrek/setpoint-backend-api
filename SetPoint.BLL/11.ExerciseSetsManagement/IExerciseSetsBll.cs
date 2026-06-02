using SetPoint.BLL._11.ExerciseSetsManagement.Dto;

namespace SetPoint.BLL._11.ExerciseSetsManagement
{
    public interface IExerciseSetsBll
    {
        Task<bool> SyncExerciseSet(ExerciseSetsDto dto);
    }
}
