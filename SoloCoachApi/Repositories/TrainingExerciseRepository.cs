using Microsoft.EntityFrameworkCore;
using SoloCoachApi.DataBase;
using SoloCoachApi.Mappers;
using SoloCoachApi.ModelDto;

namespace SoloCoachApi.Repositories
{
    public class TrainingExerciseRepository : ITrainingExerciseRepository
    {
        private readonly ApplicationContext _context;
        private readonly TrainingExerciseMapper _trainingExerciseMapper;

        public TrainingExerciseRepository(ApplicationContext context, TrainingExerciseMapper trainingExerciseMapper)
        {
            _context = context;
            _trainingExerciseMapper = trainingExerciseMapper;
        }

        public async Task<TrainingExerciseDto> GetTrainingExerciseByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID training exercise must be a positive number", nameof(id));
            }

            var trainingExercise = await _context.TrainingExercises.FindAsync(id);

            if (trainingExercise == null)
            {
                throw new KeyNotFoundException($"TrainingExercise with ID {id} does not exist");
            }

            return _trainingExerciseMapper.ToDto(trainingExercise);
        }

        public async Task<List<TrainingExerciseDto>> GetAllTrainingExercisesAsync()
        {
            var trainingExercises = await _context.TrainingExercises.ToListAsync();
            return trainingExercises.Select(_trainingExerciseMapper.ToDto).ToList();
        }

        public async Task<TrainingExerciseDto> CreateTrainingExerciseAsync(TrainingExerciseDto dto)
        {
            var entity = _trainingExerciseMapper.ToModel(dto);
            entity.IdTrainingExercise = 0;

            _context.TrainingExercises.Add(entity);
            await _context.SaveChangesAsync();

            return _trainingExerciseMapper.ToDto(entity);
        }

        public async Task<TrainingExerciseDto> UpdateTrainingExerciseAsync(TrainingExerciseDto dto)
        {
            if (dto.IdTrainingExercise <= 0)
            {
                throw new ArgumentException("ID training exercise must be a positive number", nameof(dto.IdTrainingExercise));
            }

            var existing = await _context.TrainingExercises.FindAsync(dto.IdTrainingExercise);
            if (existing == null)
            {
                throw new KeyNotFoundException($"TrainingExercise with ID {dto.IdTrainingExercise} does not exist");
            }

            existing.ExerciseId = dto.ExerciseId;
            existing.ExecutionOrder = dto.ExecutionOrder;
            existing.Sets = dto.Sets;
            existing.RestTime = dto.RestTime;
            existing.Repetitions = dto.Repetitions;

            await _context.SaveChangesAsync();

            return _trainingExerciseMapper.ToDto(existing);
        }

        public async Task DeleteTrainingExerciseAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID training exercise must be a positive number", nameof(id));
            }

            var trainingExercise = await _context.TrainingExercises.FindAsync(id);
            if (trainingExercise == null)
            {
                throw new KeyNotFoundException($"TrainingExercise with ID {id} does not exist");
            }

            _context.TrainingExercises.Remove(trainingExercise);
            await _context.SaveChangesAsync();
        }
    }
}

