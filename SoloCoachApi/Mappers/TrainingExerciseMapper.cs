using SoloCoachApi.ModelDto;
using SoloCoachApi.Models;

namespace SoloCoachApi.Mappers
{
    public class TrainingExerciseMapper
    {
        public TrainingExercise ToModel(TrainingExerciseDto dto)
        {
            return new TrainingExercise
            {
                IdTrainingExercise = dto.IdTrainingExercise,
                WorkoutId = dto.WorkoutId,
                ExerciseId = dto.ExerciseId,
                ExecutionOrder = dto.ExecutionOrder,
                Repetitions = dto.Repetitions,
                Sets = dto.Sets,
                RestTime = dto.RestTime
            };
        }

        public TrainingExerciseDto ToDto(TrainingExercise trainingExercise)
        {
            return new TrainingExerciseDto
            {
                IdTrainingExercise = trainingExercise.IdTrainingExercise,
                WorkoutId = trainingExercise.WorkoutId,
                ExerciseId = trainingExercise.ExerciseId,
                ExecutionOrder = trainingExercise.ExecutionOrder,
                Repetitions = trainingExercise.Repetitions,
                Sets = trainingExercise.Sets,
                RestTime = trainingExercise.RestTime
            };
        }
    }
}
