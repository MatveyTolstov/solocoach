using SoloCoachApi.ModelDto;
using SoloCoachApi.Models;

namespace SoloCoachApi.Mappers
{
    public class ExerciseGroupsMuscleMapper
    {
        public ExerciseGroupsMuscle ToModel(ExerciseGroupsMuscleDto dto)
        {
            return new ExerciseGroupsMuscle
            {
                IdExerciseGroupsMuscle = dto.IdExerciseGroupsMuscle,
                ExerciseId = dto.ExerciseId,
                GroupsMusclesId = dto.GroupsMusclesId
            };
        }

        public ExerciseGroupsMuscleDto ToDto(ExerciseGroupsMuscle entity)
        {
            return new ExerciseGroupsMuscleDto
            {
                IdExerciseGroupsMuscle = entity.IdExerciseGroupsMuscle,
                ExerciseId = entity.ExerciseId,
                GroupsMusclesId = entity.GroupsMusclesId
            };
        }
    }
}
