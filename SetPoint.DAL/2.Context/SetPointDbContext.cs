using Microsoft.EntityFrameworkCore;
using SetPoint.DAL._1.Entity;

namespace SetPoint.DAL._2.Context
{
    public class SetPointDbContext : DbContext
    {
        #region Fields

        private readonly string _strConfig;

        #endregion


        #region Constructors

        public SetPointDbContext(string strConfig)
        {
            _strConfig = strConfig;
        }
        //public SetPointDbContext()
        //{
        //}

        #endregion


        #region DbSets
        // --------------------------------------------------------------------------------------------------- Users, Relations and Invitations
        public DbSet<Users> Users { get; set; }
        public DbSet<UsersRelations> UsersRelations { get; set; }
        public DbSet<UsersInvitations> UsersInvitations { get; set; }
        // --------------------------------------------------------------------------------------------------- Body Measurements, Exercises, Routines, Workout Sessions
        public DbSet<BodyMeasurements> BodyMeasurements { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<MuscleGroup> MuscleGroups { get; set; }
        public DbSet<ExerciseMuscleGroup> ExerciseMuscleGroups { get; set; }
        public DbSet<Routines> Routines { get; set; }
        public DbSet<RoutineExercises> RoutineExercises { get; set; }
        public DbSet<RoutineRequests> RoutineRequests { get; set; }
        public DbSet<WorkoutSessions> WorkoutSessions { get; set; }
        public DbSet<WorkoutExercises> WorkoutExercises { get; set; }
        public DbSet<ExerciseSets> ExerciseSets { get; set; }
        // --------------------------------------------------------------------------------------------------- Feed Events, Logs
        public DbSet<FeedEvent> FeedEvents { get; set; }
        public DbSet<Logs> Logs { get; set; }
        // --------------------------------------------------------------------------------------------------------------
        #endregion


        #region Methods
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseNpgsql(_strConfig);

        }

        #endregion

    }
}
