using SetPoint.BLL._05.MuscleGroupsManagement.Dto;

namespace SetPoint.BLL._05.MuscleGroupsManagement
{
    public interface IMuscleGroupBll
    {
        Task<bool> SyncMuscleGroup(MuscleGroupDto muscleGroupDto);
        Task<bool> CreateMuscleGroup(MuscleGroupDto muscleGroupDto);
        Task<bool> UpdateMuscleGroup(MuscleGroupDto muscleGroupDto);
        Task<MuscleGroupDto?> GetMuscleGroupById(Guid id);
        Task<IEnumerable<MuscleGroupDto>> GetAllMuscleGroups();
        Task<bool> DeleteMuscleGroup(Guid id);
    }
}
