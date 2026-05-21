using SetPoint.BLL._08.RoutineExercisesManagement.Dto;

namespace SetPoint.BLL._08.RoutineExercisesManagement
{
    public interface IRoutineExercisesBll
    {
        Task<bool> SyncRoutineExercise(RoutineExerciseDto dto);
        Task<bool> CreateRoutineExercise(RoutineExerciseDto routineExerciseDto);
        Task<bool> UpdateRoutineExercise(RoutineExerciseDto routineExerciseDto);
        Task<RoutineExerciseDto?> GetRoutineExerciseById(Guid id);
        Task<IEnumerable<RoutineExerciseDto>> GetAllRoutineExercises(Guid routine);
        Task<bool> DeleteRoutineExercise(Guid id);
    }
}
