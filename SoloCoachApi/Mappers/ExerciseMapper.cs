using SoloCoachApi.ModelDto;
using SoloCoachApi.Models;

namespace SoloCoachApi.Mappers
{
    public class ExerciseMapper
    {
        public Exercise ToModel(ExerciseDto dto)
        {
            return new Exercise
            {
                IdExercise = dto.IdExercise,
                Name = dto.Name,
                Description = dto.Description,
                Complexity = dto.Complexity,
                PictureUrl = dto.PictureUrl,
                VideoUrl = dto.VideoUrl,
            };
        }

        public ExerciseDto ToDto(Exercise exercise)
        {
            return new ExerciseDto
            {
                IdExercise = exercise.IdExercise,
                Name = exercise.Name,
                Description = exercise.Description,
                Complexity = exercise.Complexity,
                PictureUrl = exercise.PictureUrl,
                VideoUrl = exercise.VideoUrl,
                MuscleGroups = exercise.ExerciseGroupsMuscles
                    .Select(eg => eg.GroupsMuscle.Name)
                    .ToList(),
            };
        }
    }
}
