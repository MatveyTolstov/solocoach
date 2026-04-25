using Microsoft.EntityFrameworkCore;
using SoloCoachApi.DataBase;
using SoloCoachApi.Mappers;
using SoloCoachApi.ModelDto;
using SoloCoachApi.Models;

namespace SoloCoachApi.Repositories
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly ApplicationContext _context;
        private readonly MetricsUserMapper _metricsMapper;
        private readonly GoalMapper _goalMapper;

        private const float MinWeightKg = 35f;
        private const float MaxWeightKg = 250f;
        private const float MaintainEpsKg = 1.0f;

        public UserProfileRepository(
            ApplicationContext context,
            MetricsUserMapper metricsMapper,
            GoalMapper goalMapper)
        {
            _context = context;
            _metricsMapper = metricsMapper;
            _goalMapper = goalMapper;
        }

        public async Task<UserProfileDto> GetProfileAsync(int userId)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Include(u => u.MetricsUser!)
                    .ThenInclude(m => m.Goal)
                .FirstOrDefaultAsync(u => u.IdUser == userId);

            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} does not exist");
            }

            return MapToProfileDto(user);
        }

        public async Task<UserProfileDto> UpdateProfileAsync(int userId, UpdateUserProfileDto dto)
        {
            var user = await _context.Users
                .Include(u => u.MetricsUser!)
                    .ThenInclude(m => m.Goal)
                .FirstOrDefaultAsync(u => u.IdUser == userId);

            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} does not exist");
            }

            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                user.Name = dto.Name.Trim();
            }

            if (!string.IsNullOrWhiteSpace(dto.Login))
            {
                var nextLogin = dto.Login.Trim();
                var loginTaken = await _context.Users
                    .AnyAsync(u => u.IdUser != userId && u.Login == nextLogin);
                if (loginTaken)
                {
                    throw new ArgumentException("Login already exists");
                }
                user.Login = nextLogin;
            }

            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                var nextEmail = dto.Email.Trim();
                var emailTaken = await _context.Users
                    .AnyAsync(u => u.IdUser != userId && u.Email == nextEmail);
                if (emailTaken)
                {
                    throw new ArgumentException("Email already exists");
                }
                user.Email = nextEmail;
            }

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                if (dto.Password.Length < 6)
                {
                    throw new ArgumentException("Password must be at least 6 characters");
                }
                user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }

            if (dto.Metrics != null)
            {
                await ApplyMetricsAsync(user, dto.Metrics);
            }

            await _context.SaveChangesAsync();

            await _context.Entry(user).Reference(u => u.MetricsUser).LoadAsync();
            if (user.MetricsUser != null)
            {
                await _context.Entry(user.MetricsUser).Reference(m => m.Goal).LoadAsync();
            }

            return MapToProfileDto(user);
        }

        private async Task ApplyMetricsAsync(User user, MetricsUserDto m)
        {
            ValidateMetrics(m);

            var goalId = await ResolveOrUpsertGoalIdAsync(user, m);

            if (!user.MetricsUserId.HasValue)
            {
                var entity = _metricsMapper.ToModel(m);
                entity.IdMetricsUser = 0;
                entity.GoalId = goalId;
                _context.MetricsUsers.Add(entity);
                await _context.SaveChangesAsync();
                user.MetricsUserId = entity.IdMetricsUser;
                user.MetricsUser = entity;
            }
            else
            {
                var existing = user.MetricsUser ?? await _context.MetricsUsers.FindAsync(user.MetricsUserId.Value);
                if (existing == null)
                {
                    throw new InvalidOperationException("Metrics user reference is broken");
                }

                existing.Height = m.Height;
                existing.Weight = m.Weight;
                existing.Age = m.Age;
                existing.Gender = m.Gender;
                existing.ExperienceLevel = m.ExperienceLevel;
                existing.ActivityLevel = m.ActivityLevel;
                existing.GoalId = goalId;
            }
        }

        private void ValidateMetrics(MetricsUserDto m)
        {
            if (m.Weight is < MinWeightKg or > MaxWeightKg)
            {
                throw new ArgumentException(
                    $"Weight must be between {MinWeightKg} and {MaxWeightKg} kg",
                    nameof(m.Weight));
            }

            if (m.TargetWeight.HasValue && (m.TargetWeight.Value is < MinWeightKg or > MaxWeightKg))
            {
                throw new ArgumentException(
                    $"TargetWeight must be between {MinWeightKg} and {MaxWeightKg} kg",
                    nameof(m.TargetWeight));
            }

            if (!string.IsNullOrWhiteSpace(m.ActivityLevel))
            {
                var a = m.ActivityLevel.Trim();
                var ok = a.Equals("Лёгкая", StringComparison.OrdinalIgnoreCase)
                         || a.Equals("Умеренная", StringComparison.OrdinalIgnoreCase)
                         || a.Equals("Интенсивная", StringComparison.OrdinalIgnoreCase);
                if (!ok) throw new ArgumentException("Invalid activityLevel", nameof(m.ActivityLevel));
            }

            if (!string.IsNullOrWhiteSpace(m.ExperienceLevel))
            {
                var e = m.ExperienceLevel.Trim();
                var ok = e.Equals("Начинающий", StringComparison.OrdinalIgnoreCase)
                         || e.Equals("Средний", StringComparison.OrdinalIgnoreCase)
                         || e.Equals("Продвинутый", StringComparison.OrdinalIgnoreCase);
                if (!ok) throw new ArgumentException("Invalid experienceLevel", nameof(m.ExperienceLevel));
            }
        }

        /// <summary>
        /// Одна цель на пользователя: если у метрик уже есть goal_id и эта запись в goals
        /// ссылается только на этого пользователя — обновляем её. Иначе (общий каталог)
        /// создаём новую персональную цель и подставляем её id.
        /// </summary>
        private async Task<int> ResolveOrUpsertGoalIdAsync(User user, MetricsUserDto m)
        {
            // 1) If client sent GoalId, ensure it exists.
            if (m.GoalId.HasValue && m.GoalId.Value > 0)
            {
                var exists = await _context.Goals.AnyAsync(g => g.IdGoal == m.GoalId.Value);
                if (!exists)
                {
                    throw new ArgumentException($"Goal with ID {m.GoalId.Value} does not exist", nameof(m.GoalId));
                }
                return m.GoalId.Value;
            }

            // 2) Personal goal from GoalType + TargetWeight (create or update one row).
            if (string.IsNullOrWhiteSpace(m.GoalType) || !m.TargetWeight.HasValue)
            {
                throw new ArgumentException(
                    "GoalId is missing. Provide either GoalId or (goalType + targetWeight).",
                    nameof(m.GoalId));
            }

            var goalType = NormalizeGoalType(m.GoalType);
            var target = m.TargetWeight.Value;

            // Logical checks vs current weight (server-side guard rail).
            // If user sets target ~= current weight, allow it for any goal type
            // (goal is immediately achieved).
            if (Math.Abs(target - m.Weight) <= MaintainEpsKg)
            {
                return await ResolveOwnedOrCreateGoalAsync(user, goalType, target);
            }

            switch (goalType)
            {
                case "Похудение":
                    if (target >= m.Weight - MaintainEpsKg)
                        throw new ArgumentException("For weight loss, targetWeight must be lower than current weight.");
                    break;
                case "Набор мышечной массы":
                    if (target <= m.Weight + MaintainEpsKg)
                        throw new ArgumentException("For muscle gain, targetWeight must be higher than current weight.");
                    break;
                case "Удержание веса":
                    if (Math.Abs(target - m.Weight) > MaintainEpsKg)
                        throw new ArgumentException("For maintenance, targetWeight must be close to current weight.");
                    break;
                case "Повышение выносливости":
                    // No strict weight constraint.
                    break;
            }

            var now = DateTime.UtcNow;

            var currentMetricsGoalId = user.MetricsUser?.GoalId;
            if (currentMetricsGoalId is > 0)
            {
                var usersWithSameGoal = await _context.MetricsUsers
                    .AsNoTracking()
                    .CountAsync(x => x.GoalId == currentMetricsGoalId.Value);

                if (usersWithSameGoal == 1)
                {
                    var owned = await _context.Goals.FindAsync(currentMetricsGoalId.Value);
                    if (owned != null)
                    {
                        owned.TypeGoal = goalType;
                        owned.TargetWeight = target;
                        owned.DateEnd = now.AddDays(90);
                        owned.Status = "Active";
                        await _context.SaveChangesAsync();
                        return owned.IdGoal;
                    }
                }
            }

            var entity = new Goal
            {
                IdGoal = 0,
                TypeGoal = goalType,
                TargetWeight = target,
                DateStart = now,
                DateEnd = now.AddDays(90),
                Status = "Active"
            };

            _context.Goals.Add(entity);
            await _context.SaveChangesAsync();

            return entity.IdGoal;
        }

        private async Task<int> ResolveOwnedOrCreateGoalAsync(User user, string goalType, float target)
        {
            var now = DateTime.UtcNow;

            var currentMetricsGoalId = user.MetricsUser?.GoalId;
            if (currentMetricsGoalId is > 0)
            {
                var usersWithSameGoal = await _context.MetricsUsers
                    .AsNoTracking()
                    .CountAsync(x => x.GoalId == currentMetricsGoalId.Value);

                if (usersWithSameGoal == 1)
                {
                    var owned = await _context.Goals.FindAsync(currentMetricsGoalId.Value);
                    if (owned != null)
                    {
                        owned.TypeGoal = goalType;
                        owned.TargetWeight = target;
                        owned.DateEnd = now.AddDays(90);
                        owned.Status = "Active";
                        await _context.SaveChangesAsync();
                        return owned.IdGoal;
                    }
                }
            }

            var entity = new Goal
            {
                IdGoal = 0,
                TypeGoal = goalType,
                TargetWeight = target,
                DateStart = now,
                DateEnd = now.AddDays(90),
                Status = "Active"
            };

            _context.Goals.Add(entity);
            await _context.SaveChangesAsync();

            return entity.IdGoal;
        }

        private static string NormalizeGoalType(string raw)
        {
            var s = raw.Trim();
            if (s.Equals("Похудение", StringComparison.OrdinalIgnoreCase)) return "Похудение";
            if (s.Equals("Набор мышечной массы", StringComparison.OrdinalIgnoreCase)) return "Набор мышечной массы";
            if (s.Equals("Повышение выносливости", StringComparison.OrdinalIgnoreCase)) return "Повышение выносливости";
            if (s.Equals("Удержание веса", StringComparison.OrdinalIgnoreCase)) return "Удержание веса";

            throw new ArgumentException(
                "Invalid goalType. Allowed: Похудение, Набор мышечной массы, Повышение выносливости, Удержание веса.",
                nameof(raw));
        }

        private UserProfileDto MapToProfileDto(User user)
        {
            MetricsUserDto? metricsDto = null;
            GoalDto? goalDto = null;

            if (user.MetricsUser != null)
            {
                metricsDto = _metricsMapper.ToDto(user.MetricsUser);
                if (user.MetricsUser.Goal != null)
                {
                    goalDto = _goalMapper.ToDto(user.MetricsUser.Goal);
                }
            }

            return new UserProfileDto
            {
                IdUser = user.IdUser,
                Name = user.Name,
                Login = user.Login,
                Email = user.Email,
                MetricsUserId = user.MetricsUserId,
                Metrics = metricsDto,
                CurrentGoal = goalDto
            };
        }
    }
}
