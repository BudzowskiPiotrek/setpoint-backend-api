using SetPoint.BLL._05.MuscleGroupsManagement.Dto;

namespace SetPoint.BLL._05.MuscleGroupsManagement
{
    public interface IMuscleGroupBll
    {
        Task<bool> SyncMuscleGroup(MuscleGroupDto muscleGroupDto);
    }
}
