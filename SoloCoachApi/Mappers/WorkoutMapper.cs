using SoloCoachApi.ModelDto;
using SoloCoachApi.Models;

namespace SoloCoachApi.Mappers
{
    public class WorkoutMapper
    {
        private readonly ExerciseMapper _exerciseMapper;

        public WorkoutMapper(ExerciseMapper exerciseMapper)
        {
            _exerciseMapper = exerciseMapper;
        }

        public Workout ToModel(WorkoutDto dto)
        {
            return new Workout
            {
                IdWorkout = dto.IdWorkout,
                Name = dto.Name,
                Description = dto.Description,
                Duration = dto.Duration,
                Complexity = dto.Complexity,
                TypeWorkout = dto.TypeWorkout
            };
        }

        public WorkoutDto ToDto(Workout workout, IReadOnlyList<TrainingExercise>? trainingExercises = null)
        {
            var dto = new WorkoutDto
            {
                IdWorkout = workout.IdWorkout,
                Name = workout.Name,
                Description = workout.Description,
                Duration = workout.Duration,
                Complexity = workout.Complexity,
                TypeWorkout = workout.TypeWorkout
            };

            if (trainingExercises is { Count: > 0 })
            {
                dto.TrainingExercises = trainingExercises
                    .OrderBy(te => te.ExecutionOrder)
                    .Select(te => new WorkoutTrainingExerciseDto
                    {
                        IdTrainingExercise = te.IdTrainingExercise,
                        ExecutionOrder = te.ExecutionOrder,
                        Repetitions = te.Repetitions,
                        Sets = te.Sets,
                        RestTime = te.RestTime,
                        Exercise = _exerciseMapper.ToDto(te.Exercise)
                    })
                    .ToList();
            }

            return dto;
        }
    }
}
