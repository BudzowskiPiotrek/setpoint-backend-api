using AutoMapper;
using SetPoint.BLL._02.UserRelationManagement.Dto;
using SetPoint.BLL._02.UsersInvitationManagement.Dto;
using SetPoint.BLL._02.UsersManagement;
using SetPoint.BLL._02.UsersManagement.Dto;
using SetPoint.BLL._03.BodyMeasurementsManagement.Dto;
using SetPoint.BLL._04.ExercisesManagement.Dto;
using SetPoint.BLL._05.MuscleGroupsManagement.Dto;
using SetPoint.BLL._06.ExerciseMuscleManagement.Dto;
using SetPoint.BLL._07.RoutineRequestManagement.Dto;
using SetPoint.BLL._07.RoutinesManagement.Dto;
using SetPoint.BLL._08.RoutineExercisesManagement.Dto;
using SetPoint.BLL._09.WorkoutSessionsManagement.Dto;
using SetPoint.BLL._10.WorkoutExercisesManagement.Dto;
using SetPoint.BLL._11.ExerciseSetsManagement.Dto;
using SetPoint.BLL._12.FeedEventManagement.Dto;
using SetPoint.DAL._1.Entity;

namespace SetPoint.BLL._0.Common
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Users, UserReadDto>().ReverseMap();
            CreateMap<UserDto, Users>();
            CreateMap<UsersRelations, UserRelationDto>().ReverseMap();
            CreateMap<UsersInvitations, UsersInvitationDto>().ReverseMap();
            CreateMap<BodyMeasurements, BodyMeasurementsDto>().ReverseMap();
            CreateMap<Exercise, ExercisesDto>().ReverseMap();
            CreateMap<MuscleGroup, MuscleGroupDto>().ReverseMap();
            CreateMap<ExerciseMuscleGroup, ExerciseMuscleDto>().ReverseMap();
            CreateMap<Routines, RoutineDto>().ReverseMap();
            CreateMap<RoutineRequests, RoutineRequestDto>();
            CreateMap<RoutineExercises, RoutineExerciseDto>().ReverseMap();
            CreateMap<WorkoutSessions, WorkoutSessionsDto>().ReverseMap();
            CreateMap<WorkoutExercises, WorkoutExercisesDto>().ReverseMap();
            CreateMap<ExerciseSets, ExerciseSetsDto>().ReverseMap();
            CreateMap<FeedEvent, FeedEventDto>().ReverseMap();
        }
    }
}
