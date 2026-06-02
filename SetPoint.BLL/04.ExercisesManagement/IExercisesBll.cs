using SetPoint.BLL._04.ExercisesManagement.Dto;

namespace SetPoint.BLL._04.ExercisesManagement
{
    public interface IExercisesBll
    {
        Task<bool> SyncExercise(ExercisesDto exerciseDto);
    }
}
