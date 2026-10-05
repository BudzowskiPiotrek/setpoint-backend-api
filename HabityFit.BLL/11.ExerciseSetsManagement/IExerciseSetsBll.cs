using HabityFit.BLL._11.ExerciseSetsManagement.Dto;

namespace HabityFit.BLL._11.ExerciseSetsManagement
{
    public interface IExerciseSetsBll
    {
        Task<bool> SyncExerciseSet(ExerciseSetsDto dto);
    }
}
