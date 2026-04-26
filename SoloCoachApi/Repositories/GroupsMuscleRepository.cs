using Microsoft.EntityFrameworkCore;
using SoloCoachApi.DataBase;
using SoloCoachApi.Mappers;
using SoloCoachApi.ModelDto;

namespace SoloCoachApi.Repositories
{
    public class GroupsMuscleRepository : IGroupsMuscleRepository
    {
        private readonly ApplicationContext _context;
        private readonly GroupsMuscleMapper _groupsMuscleMapper;

        public GroupsMuscleRepository(ApplicationContext context, GroupsMuscleMapper groupsMuscleMapper)
        {
            _context = context;
            _groupsMuscleMapper = groupsMuscleMapper;
        }

        public async Task<GroupsMuscleDto> GetGroupsMuscleByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID группы мышц должен быть положительным числом", nameof(id));
            }

            var group = await _context.GroupsMuscles.FindAsync(id);

            if (group == null)
            {
                throw new KeyNotFoundException($"Группа мышц с ID {id} не существует");
            }

            return _groupsMuscleMapper.ToDto(group);
        }

        public async Task<List<GroupsMuscleDto>> GetAllGroupsMusclesAsync()
        {
            var groups = await _context.GroupsMuscles.ToListAsync();
            return groups.Select(_groupsMuscleMapper.ToDto).ToList();
        }

        public async Task<GroupsMuscleDto> CreateGroupsMuscleAsync(GroupsMuscleDto dto)
        {
            var entity = _groupsMuscleMapper.ToModel(dto);
            entity.IdGroupsMuscle = 0;

            _context.GroupsMuscles.Add(entity);
            await _context.SaveChangesAsync();

            return _groupsMuscleMapper.ToDto(entity);
        }

        public async Task<GroupsMuscleDto> UpdateGroupsMuscleAsync(GroupsMuscleDto dto)
        {
            if (dto.IdGroupsMuscle <= 0)
            {
                throw new ArgumentException("ID группы мышц должен быть положительным числом", nameof(dto.IdGroupsMuscle));
            }

            var existing = await _context.GroupsMuscles.FindAsync(dto.IdGroupsMuscle);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Группа мышц с ID {dto.IdGroupsMuscle} не существует");
            }

            existing.Name = dto.Name;

            await _context.SaveChangesAsync();

            return _groupsMuscleMapper.ToDto(existing);
        }

        public async Task DeleteGroupsMuscleAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID группы мышц должен быть положительным числом", nameof(id));
            }

            var group = await _context.GroupsMuscles.FindAsync(id);
            if (group == null)
            {
                throw new KeyNotFoundException($"Группа мышц с ID {id} не существует");
            }

            _context.GroupsMuscles.Remove(group);
            await _context.SaveChangesAsync();
        }
    }
}