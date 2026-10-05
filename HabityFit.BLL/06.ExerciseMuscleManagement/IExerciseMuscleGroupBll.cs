using HabityFit.BLL._06.ExerciseMuscleManagement.Dto;

namespace HabityFit.BLL._06.ExerciseMuscleManagement
{
    public interface IExerciseMuscleGroupBll
    {
        Task<bool> SyncExerciseMuscleGroup(ExerciseMuscleDto dto);
    }
}
