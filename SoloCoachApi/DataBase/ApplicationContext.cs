using Microsoft.EntityFrameworkCore;
using SoloCoachApi.Models;

namespace SoloCoachApi.DataBase
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions options) : base(options)
        {
        }

        protected ApplicationContext()
        {
        }

        public DbSet<ExerciseGroupsMuscle> ExercisesGroups { get; set; } 
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<Goal> Goals { get; set; }
        public DbSet<GroupsMuscle> GroupsMuscles { get; set; }
        public DbSet<MetricsUser> MetricsUsers { get; set; }
        public DbSet<PlanWorkout> PlanWorkouts { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<TrainingExercise> TrainingExercises { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Workout> Workouts { get; set; }
        public DbSet<WorkoutCalendar> WorkoutCalendars { get; set; }
        public DbSet<WorkoutUser> WorkoutUser { get; set; }
        public DbSet<WorkoutUserLog> WorkoutUserLogs { get; set; }
        public DbSet<ApplicationLog> ApplicationLogs { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
        public DbSet<EmailChangeCode> EmailChangeCodes { get; set; }
    }
}
