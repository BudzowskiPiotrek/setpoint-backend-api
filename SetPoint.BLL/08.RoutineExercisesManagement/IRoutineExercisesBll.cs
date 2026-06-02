using SetPoint.BLL._08.RoutineExercisesManagement.Dto;

namespace SetPoint.BLL._08.RoutineExercisesManagement
{
    public interface IRoutineExercisesBll
    {
        Task<bool> SyncRoutineExercise(RoutineExerciseDto dto);
    }
}
