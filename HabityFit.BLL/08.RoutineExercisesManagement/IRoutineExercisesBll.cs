using HabityFit.BLL._08.RoutineExercisesManagement.Dto;

namespace HabityFit.BLL._08.RoutineExercisesManagement
{
    public interface IRoutineExercisesBll
    {
        Task<bool> SyncRoutineExercise(RoutineExerciseDto dto);
    }
}
