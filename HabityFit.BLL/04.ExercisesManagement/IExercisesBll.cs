using HabityFit.BLL._04.ExercisesManagement.Dto;

namespace HabityFit.BLL._04.ExercisesManagement
{
    public interface IExercisesBll
    {
        Task<bool> SyncExercise(ExercisesDto exerciseDto);
    }
}
