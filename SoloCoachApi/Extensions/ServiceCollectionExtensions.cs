using Microsoft.Extensions.DependencyInjection;
using SoloCoachApi.Mappers;
using SoloCoachApi.Repositories;
using SoloCoachApi.Services;

namespace SoloCoachApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddProjectDependencies(this IServiceCollection services)
        {
            // Mappers
            services.AddScoped<RoleMapper>();
            services.AddScoped<UserMapper>();
            services.AddScoped<WorkoutMapper>();
            services.AddScoped<GoalMapper>();
            services.AddScoped<ExerciseMapper>();
            services.AddScoped<GroupsMuscleMapper>();
            services.AddScoped<ExerciseGroupsMuscleMapper>();
            services.AddScoped<MetricsUserMapper>();
            services.AddScoped<PlanWorkoutMapper>();
            services.AddScoped<TrainingExerciseMapper>();
            services.AddScoped<WorkoutCalendarMapper>();
            services.AddScoped<WorkoutUserMapper>();
            services.AddScoped<WorkoutUserLogMapper>();
            services.AddScoped<ApplicationLogMapper>();

            // Repositories
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IWorkoutRepository, WorkoutRepository>();
            services.AddScoped<IGoalRepository, GoalRepository>();
            services.AddScoped<IExerciseRepository, ExerciseRepository>();
            services.AddScoped<IGroupsMuscleRepository, GroupsMuscleRepository>();
            services.AddScoped<IExerciseGroupsMuscleRepository, ExerciseGroupsMuscleRepository>();
            services.AddScoped<IMetricsUserRepository, MetricsUserRepository>();
            services.AddScoped<IPlanWorkoutRepository, PlanWorkoutRepository>();
            services.AddScoped<ITrainingExerciseRepository, TrainingExerciseRepository>();
            services.AddScoped<IWorkoutCalendarRepository, WorkoutCalendarRepository>();
            services.AddScoped<IWorkoutUserRepository, WorkoutUserRepository>();
            services.AddScoped<IWorkoutUserLogRepository, WorkoutUserLogRepository>();
            services.AddScoped<IUserProfileRepository, UserProfileRepository>();
            services.AddScoped<IApplicationLogRepository, ApplicationLogRepository>();

            // Services
            services.AddScoped<UserService>();
            services.AddScoped<RoleService>();
            services.AddScoped<WorkoutService>();
            services.AddScoped<GoalService>();
            services.AddScoped<ExerciseService>();
            services.AddScoped<GroupsMuscleService>();
            services.AddScoped<ExerciseGroupsMuscleService>();
            services.AddScoped<MetricsUserService>();
            services.AddScoped<PlanWorkoutService>();
            services.AddScoped<TrainingExerciseService>();
            services.AddScoped<WorkoutCalendarService>();
            services.AddScoped<WorkoutUserService>();
            services.AddScoped<WorkoutUserLogService>();
            services.AddScoped<WorkoutSessionService>();
            services.AddScoped<JwtService>();
            services.AddScoped<ILoggingService, LoggingService>();
            services.AddScoped<ProfileService>();
            services.AddScoped<ApplicationLogService>();
            services.AddScoped<IEmailService, SmtpEmailService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddHttpClient<AiService>();
            services.AddSingleton<S3Service>();

            return services;
        }
    }
}

