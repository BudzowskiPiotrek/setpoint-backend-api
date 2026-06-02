using SetPoint.BLL._06.ExerciseMuscleManagement.Dto;

namespace SetPoint.BLL._06.ExerciseMuscleManagement
{
    public interface IExerciseMuscleGroupBll
    {
        Task<bool> SyncExerciseMuscleGroup(ExerciseMuscleDto dto);
    }
}
