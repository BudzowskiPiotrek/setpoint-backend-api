using HabityFit.BLL._05.MuscleGroupsManagement.Dto;

namespace HabityFit.BLL._05.MuscleGroupsManagement
{
    public interface IMuscleGroupBll
    {
        Task<bool> SyncMuscleGroup(MuscleGroupDto muscleGroupDto);
    }
}
