using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SetPoint.BLL._0.Sync.Dto;
using SetPoint.BLL._02.UserRelationManagement;
using SetPoint.BLL._02.UserRelationManagement.Dto;
using SetPoint.BLL._02.UsersInvitationManagement;
using SetPoint.BLL._02.UsersManagement;
using SetPoint.BLL._02.UsersManagement.Dto;
using SetPoint.BLL._03.BodyMeasurementsManagement.Dto;
using SetPoint.BLL._04.ExercisesManagement;
using SetPoint.BLL._04.ExercisesManagement.Dto;
using SetPoint.BLL._05.MuscleGroupsManagement;
using SetPoint.BLL._05.MuscleGroupsManagement.Dto;
using SetPoint.BLL._06.ExerciseMuscleManagement;
using SetPoint.BLL._06.ExerciseMuscleManagement.Dto;
using SetPoint.BLL._07.RoutineRequestManagement;
using SetPoint.BLL._07.RoutineRequestManagement.Dto;
using SetPoint.BLL._07.RoutinesManagement;
using SetPoint.BLL._07.RoutinesManagement.Dto;
using SetPoint.BLL._08.RoutineExercisesManagement;
using SetPoint.BLL._08.RoutineExercisesManagement.Dto;
using SetPoint.BLL._09.WorkoutSessionsManagement;
using SetPoint.BLL._09.WorkoutSessionsManagement.Dto;
using SetPoint.BLL._1.Security;
using SetPoint.BLL._10.WorkoutExercisesManagement;
using SetPoint.BLL._10.WorkoutExercisesManagement.Dto;
using SetPoint.BLL._11.ExerciseSetsManagement;
using SetPoint.BLL._11.ExerciseSetsManagement.Dto;
using SetPoint.BLL._12.FeedEventManagement.Dto;
using SetPoint.DAL._1.Entity;
using SetPoint.DAL._2.Context;


namespace SetPoint.BLL._0.Sync
{
    public class SyncService : ISyncService
    {
        #region Fields
        private readonly IBodyMeasurementsBll _bodyBll;
        private readonly IMuscleGroupBll _muscleGroupBll;
        private readonly IExercisesBll _exercisesBll;
        private readonly IExerciseMuscleGroupBll _exerciseMuscleBll;
        private readonly IRoutineBll _routineBll;
        private readonly IRoutineExercisesBll _routineExerciseBll;
        private readonly IWorkoutSessionsBll _workoutSessionBll;
        private readonly IWorkoutExercisesBll _workoutExerciseBll;
        private readonly IExerciseSetsBll _exerciseSetBll;
        private readonly IUserBll _userBll;
        private readonly IUserRelationBll _userRelationBll;
        private readonly IUsersInvitationBll _usersInvitationBll;
        private readonly IRoutineRequestBll _routineRequestBll;

        private readonly ITokenService _tokenService;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly SetPointDbContext _context;
        #endregion


        #region Constructors
        public SyncService(
            ITokenService tokenService,
            SetPointDbContext context,
            IConfiguration config,
            IBodyMeasurementsBll bodyBll,
            IMuscleGroupBll muscleGroupBll,
            IExercisesBll exercisesBll,
            IExerciseMuscleGroupBll exerciseMuscleBll,
            IRoutineBll routineBll,
            IRoutineExercisesBll routineExerciseBll,
            IWorkoutSessionsBll workoutSessionBll,
            IWorkoutExercisesBll workoutExerciseBll,
            IExerciseSetsBll exerciseSetBll,
            IUserBll userBll,
            IUserRelationBll userRelationBll,
            IUsersInvitationBll usersInvitationBll,
            IRoutineRequestBll routineRequestBll,
            IMapper mapper)
        {
            _tokenService = tokenService;
            _context = context;
            _config = config;
            _bodyBll = bodyBll;
            _muscleGroupBll = muscleGroupBll;
            _exercisesBll = exercisesBll;
            _exerciseMuscleBll = exerciseMuscleBll;
            _routineBll = routineBll;
            _routineExerciseBll = routineExerciseBll;
            _workoutSessionBll = workoutSessionBll;
            _workoutExerciseBll = workoutExerciseBll;
            _exerciseSetBll = exerciseSetBll;
            _userBll = userBll;
            _userRelationBll = userRelationBll;
            _usersInvitationBll = usersInvitationBll;
            _routineRequestBll = routineRequestBll;
            _mapper = mapper;
        }

        #endregion


        #region Methods
        public async Task<SyncErrorDetail> ProcessPush(SyncPayloadDto payload, Guid userId)
        {
            var response = new SyncErrorDetail
            {
                ItemId = new List<string>(),
                Success = new List<bool>()
            };

            // --- Procesamiento de todas las colecciones ---
            await ProcessCollection(payload.Users, _userBll.SyncUser, x => x.Id.ToString(), response, x => x.Id = userId);
            await ProcessCollection(payload.BodyMeasurements, _bodyBll.SyncBody, x => x.Id.ToString(), response, x => x.IdUser = userId);
            await ProcessCollection(payload.MuscleGroups, _muscleGroupBll.SyncMuscleGroup, x => x.Id.ToString(), response);
            await ProcessCollection(payload.Exercises, _exercisesBll.SyncExercise, x => x.Id.ToString(), response);
            await ProcessCollection(payload.ExerciseMuscleGroups, _exerciseMuscleBll.SyncExerciseMuscleGroup, x => x.Id.ToString(), response);
            await ProcessCollection(payload.Routines, _routineBll.SyncRoutine, x => x.Id.ToString(), response, x => x.UserId = userId);
            await ProcessCollection(payload.RoutineExercises, _routineExerciseBll.SyncRoutineExercise, x => x.Id.ToString(), response);
            await ProcessCollection(payload.WorkoutSessions, _workoutSessionBll.SyncWorkoutSession, x => x.Id.ToString(), response, x => x.UserId = userId);
            await ProcessCollection(payload.WorkoutExercises, _workoutExerciseBll.SyncWorkoutExercise, x => x.Id.ToString(), response);
            await ProcessCollection(payload.ExerciseSets, _exerciseSetBll.SyncExerciseSet, x => x.Id.ToString(), response);
            await ProcessCollection(payload.UserRelations, _userRelationBll.SyncUserRelation, x => x.Id.ToString(), response);
            await ProcessCollection(payload.RoutinesRequests, _routineRequestBll.SyncRoutineRequest, x => x.Id.ToString(), response);
            await ProcessCollection(payload.UserInvitations, _usersInvitationBll.CreateAndSendInvitationAsync, x => x.Id.ToString(), response);

            // --- Logging ---
            await LogSyncResult(userId, response);

            return response;
        }

        public async Task<SyncPayloadDto?> ProcessPull(PullRequestDto request, Guid userId)
        {
            var response = new SyncPayloadDto();
            // obtener dto de usuario para generar token
            /////////////////////////////////// BODY
            var bodyEntities = await _context.BodyMeasurements
                .Where(b => b.IdUser == request.UserId && b.UpdatedAt > request.LastSync).ToListAsync();

            response.BodyMeasurements = bodyEntities
                .Select(b => _mapper.Map<BodyMeasurementsDto>(b)).ToList();

            /////////////////////////////////// MUSCLE GROUPS
            var muscleEntities = await _context.MuscleGroups
                .Where(m => m.UpdatedAt > request.LastSync).ToListAsync();

            response.MuscleGroups = muscleEntities
                .Select(m => _mapper.Map<MuscleGroupDto>(m)).ToList();

            /////////////////////////////////// EXERCISES
            var exerciseEntities = await _context.Exercises
                .Where(e => e.UpdatedAt > request.LastSync).ToListAsync();

            response.Exercises = exerciseEntities
                .Select(e => _mapper.Map<ExercisesDto>(e)).ToList();

            /////////////////////////////////// EXERCISE-MUSCLE
            var exerciseMuscleEntities = await _context.ExerciseMuscleGroups
                .Where(em => em.UpdatedAt > request.LastSync).ToListAsync();

            response.ExerciseMuscleGroups = exerciseMuscleEntities
                .Select(em => _mapper.Map<ExerciseMuscleDto>(em)).ToList();

            /////////////////////////////////// ROUTINES
            var routines = await _context.Routines
                .Where(r => r.UserId == request.UserId && r.UpdatedAt > request.LastSync).ToListAsync();

            response.Routines = routines
                .Select(r => _mapper.Map<RoutineDto>(r)).ToList();

            /////////////////////////////////// ROUTINE EXERCISES
            var routineIds = routines.Select(r => r.Id).ToList();

            var routineExEntities = await _context.RoutineExercises
                .Where(re => re.UpdatedAt > request.LastSync && routineIds.Contains(re.RoutineId)).ToListAsync();

            response.RoutineExercises = routineExEntities
                .Select(re => _mapper.Map<RoutineExerciseDto>(re)).ToList();

            /////////////////////////////////// WORKOUT SESSIONS
            var sessions = await _context.WorkoutSessions
                .Where(ws => ws.UserId == request.UserId && ws.UpdatedAt > request.LastSync).ToListAsync();

            response.WorkoutSessions = sessions
                .Select(ws => _mapper.Map<WorkoutSessionsDto>(ws)).ToList();

            /////////////////////////////////// WORKOUT EXERCISES
            var sessionIds = sessions.Select(ws => ws.Id).ToList();

            var workoutExEntities = await _context.WorkoutExercises
                .Where(we => we.UpdatedAt > request.LastSync && sessionIds.Contains(we.SessionId)).ToListAsync();

            response.WorkoutExercises = workoutExEntities
                .Select(we => _mapper.Map<WorkoutExercisesDto>(we)).ToList();

            /////////////////////////////////// SETS
            var workoutExIds = workoutExEntities.Select(we => we.Id).ToList();

            var setEntities = await _context.ExerciseSets
                .Where(es => es.UpdatedAt > request.LastSync && workoutExIds.Contains(es.WorkoutExerciseId)).ToListAsync();

            response.ExerciseSets = setEntities
                .Select(es => _mapper.Map<ExerciseSetsDto>(es)).ToList();

            /////////////////////////////////// USER RELATIONS
            var relationEntities = await _context.UsersRelations
                .Where(ur => (ur.UserId == request.UserId || ur.FriendId == request.UserId)
                             && ur.UpdatedAt > request.LastSync).ToListAsync();

            response.UserRelations = relationEntities
                .Select(ur => _mapper.Map<UserRelationDto>(ur)).ToList();

            /////////////////////////////////// USERS (SELF + FRIENDS)
            var targetUserIds = new HashSet<Guid>();

            // ADD = If the user updated themselves, if not, nothing at all!
            var selfUpdated = await _context.Users.AnyAsync(u => u.Id == request.UserId && u.UpdatedAt > request.LastSync);
            if (selfUpdated) targetUserIds.Add(request.UserId);

            // ADD = The whole list of new contacts pending/accepted
            foreach (var ur in relationEntities)
                if (ur.Status != RelationStatus.Rejected)
                    targetUserIds.Add(ur.UserId == request.UserId ? ur.FriendId : ur.UserId);
            #region UPDATED FRIENDS (ONLY IF USER UPDATED THEIR PROFILE)
            //var updatedFriendIds = await context.UsersRelations
            //    .Where(ur => (ur.UserId == request.UserId || ur.FriendId == request.UserId) && ur.Status != RelationStatus.Rejected)
            //    .Select(ur => ur.UserId == request.UserId ? ur.FriendId : ur.UserId)
            //    .Distinct()
            //    .Join(context.Users.Where(u => u.UpdatedAt > request.LastSync), friendId => friendId, user => user.Id, (friendId, user) => friendId).ToListAsync();

            //// ADD = All updated profiles from our contacts
            //foreach (var id in updatedFriendIds)
            //    targetUserIds.Add(id);
            #endregion
            var userEntities = await _context.Users.Where(u => targetUserIds.Contains(u.Id)).ToListAsync();

            response.Users = userEntities
                .Select(u => _mapper.Map<UserReadDto>(u)).ToList();

            /////////////////////////////////// ROUTINE REQUESTS
            var routineReqEntities = await _context.RoutineRequests
                .Where(rr => (rr.SenderId == request.UserId || rr.ReceiverId == request.UserId)
                             && rr.UpdatedAt > request.LastSync).ToListAsync();

            response.RoutinesRequests = routineReqEntities
                .Select(rr => _mapper.Map<RoutineRequestDto>(rr)).ToList();

            ////////////////////////////////// FEED EVENT
            var feedEvent = await _context.FeedEvents
                .Where(fe => fe.UserId == null && fe.UpdatedAt > request.LastSync).ToListAsync();

            response.FeedEvents = feedEvent
                .Select(fe => _mapper.Map<FeedEventDto>(fe)).ToList();

            ////////////////////////////////// TOKEN GENERATION 
            var userReadDto = await _userBll.GetUserById(userId);
            if (userReadDto != null)
            {
                response.Token = _tokenService.CreateToken(userReadDto);
            }

            return response;
        }
        #endregion


        #region Private Helpers
        private async Task ProcessCollection<T>(
            List<T>? collection, Func<T, Task<bool>> syncFunc,
            Func<T, string> idSelector, SyncErrorDetail response,
            Action<T>? setUserId = null)
        {
            if (collection == null || !collection.Any()) return;

            foreach (var item in collection)
            {
                string id = idSelector(item);
                try
                {
                    setUserId?.Invoke(item);

                    bool result = await syncFunc(item);

                    response.ItemId.Add(id);
                    response.Success.Add(result);
                }
                catch (Exception e)
                {
                    response.ItemId.Add(id);
                    response.Success.Add(false);
                }
            }
        }

        private async Task LogSyncResult(Guid userId, SyncErrorDetail response)
        {
            int total = response.ItemId.Count;
            if (total == 0) return;

            int successful = response.Success.Count(s => s);
            int failed = total - successful;

            var log = new Logs
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                UserId = userId,
                Type = $"[INFO] [SYNC] {total} items processed | Success: {successful} | Fail: {failed}"
            };

            await _context.Logs.AddAsync(log);
            await _context.SaveChangesAsync();
        }
        #endregion
    }
}

