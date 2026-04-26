using Microsoft.EntityFrameworkCore;
using SoloCoachApi.DataBase;
using SoloCoachApi.Mappers;
using SoloCoachApi.ModelDto;

namespace SoloCoachApi.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationContext _context;
        private readonly RoleMapper _roleMapper;

        public RoleRepository(ApplicationContext context, RoleMapper roleMapper)
        {
            _context = context;
            _roleMapper = roleMapper;
        }

        public async Task<RoleDto> GetRoleByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID роли должен быть положительным числом", nameof(id));
            }

            var role = await _context.Roles.FindAsync(id);

            if (role == null)
            {
                throw new KeyNotFoundException($"Роль с ID {id} не существует");
            }

            return _roleMapper.ToDto(role);
        }

        public async Task<List<RoleDto>> GetAllRolesAsync()
        {
            var roles = await _context.Roles.ToListAsync();
            return roles.Select(_roleMapper.ToDto).ToList();
        }

        public async Task<RoleDto> CreateRoleAsync(RoleDto dto)
        {
            var entity = _roleMapper.ToModel(dto);

            if (await _context.Roles.AnyAsync(r => r.RoleName == dto.RoleName))
                throw new InvalidOperationException($"Роль с названием '{dto.RoleName}' уже существует");

            _context.Roles.Add(entity);
            await _context.SaveChangesAsync();

            return _roleMapper.ToDto(entity);
        }

        public async Task<RoleDto> UpdateRoleAsync(RoleDto dto)
        {
            if (dto.IdRole <= 0)
            {
                throw new ArgumentException("ID роли должен быть положительным числом", nameof(dto.IdRole));
            }

            var existing = await _context.Roles.FindAsync(dto.IdRole);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Роль с ID {dto.IdRole} не существует");
            }

            if (await _context.Roles.AnyAsync(r => r.RoleName == dto.RoleName && r.IdRole != dto.IdRole))
                throw new InvalidOperationException($"Роль с названием '{dto.RoleName}' уже существует");

            existing.RoleName = dto.RoleName;

            await _context.SaveChangesAsync();

            return _roleMapper.ToDto(existing);
        }

        public async Task DeleteRoleAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID роли должен быть положительным числом", nameof(id));
            }

            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                throw new KeyNotFoundException($"Роль с ID {id} не существует");
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
        }
    }
}