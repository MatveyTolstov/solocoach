using Microsoft.EntityFrameworkCore;
using SoloCoachApi.DataBase;
using SoloCoachApi.Mappers;
using SoloCoachApi.ModelDto;

namespace SoloCoachApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationContext _context;
        private readonly UserMapper _userMapper;

        public UserRepository(ApplicationContext context, UserMapper userMapper)
        {
            _context = context;
            _userMapper = userMapper;
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID user must be a positive number", nameof(id));
            }

            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {id} does not exist");
            }

            return _userMapper.ToDto(user);
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = await _context.Users.ToListAsync();
            return users.Select(_userMapper.ToDto).ToList();
        }

        public async Task<UserDto> CreateUserAsync(UserDto dto)
        {
            var entity = _userMapper.ToModel(dto);
            entity.IdUser = 0;

            // Если пароль предоставлен, хешируем его
            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                entity.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }

            _context.Users.Add(entity);
            await _context.SaveChangesAsync();

            return _userMapper.ToDto(entity);
        }

        public async Task<UserDto> UpdateUserAsync(UserDto dto)
        {
            if (dto.IdUser <= 0)
            {
                throw new ArgumentException("ID user must be a positive number", nameof(dto.IdUser));
            }

            var existing = await _context.Users.FindAsync(dto.IdUser);
            if (existing == null)
            {
                throw new KeyNotFoundException($"User with ID {dto.IdUser} does not exist");
            }

            existing.Name = dto.Name;
            existing.Login = dto.Login;
            existing.Email = dto.Email;
            existing.RoleId = dto.RoleId;
            existing.MetricsUserId = dto.MetricsUserId;

            // Если пароль предоставлен, обновляем его
            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                existing.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }

            await _context.SaveChangesAsync();

            return _userMapper.ToDto(existing);
        }

        public async Task DeleteUserAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID user must be a positive number", nameof(id));
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {id} does not exist");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsEmailTakenAsync(int id, string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email && u.IdUser != id);
        }
    }
}

