export const MANAGER_RESOURCE_KEYS = [
  'workout',
  'exercise',
  'trainingExercise',
  'groupsMuscle',
  'exerciseGroupsMuscle',
]

export const ADMIN_DATABASE_KEYS = [
  'role',
  'metricsUser',
  'workout',
  'exercise',
  'trainingExercise',
  'groupsMuscle',
  'exerciseGroupsMuscle',
  'workoutCalendarAdmin',
  'workoutUserAdmin',
  'workoutUserLogAdmin',
]

export const ENTITY_CONFIGS = {
  role: {
    key: 'role',
    title: 'Роли',
    apiPath: '/api/Role',
    idField: 'idRole',
    tableColumns: [{ key: 'roleName', label: 'Название роли' }],
    formFields: [
      { key: 'roleName', label: 'Название роли', type: 'text', required: true },
    ],
  },
  goal: {
    key: 'goal',
    title: 'Цели',
    apiPath: '/api/Goal',
    idField: 'idGoal',
    tableColumns: [
      { key: 'typeGoal', label: 'Тип цели' },
      { key: 'targetWeight', label: 'Целевой вес' },
      { key: 'status', label: 'Статус' },
      { key: 'dateStart', label: 'Старт', type: 'datetime-local' },
      { key: 'dateEnd', label: 'Финиш', type: 'datetime-local' },
    ],
    formFields: [
      { key: 'typeGoal', label: 'Тип цели', type: 'text', required: true },
      {
        key: 'targetWeight',
        label: 'Целевой вес (кг)',
        type: 'number',
        step: '0.1',
        required: true,
      },
      {
        key: 'dateStart',
        label: 'Дата начала',
        type: 'datetime-local',
        required: true,
      },
      {
        key: 'dateEnd',
        label: 'Дата окончания',
        type: 'datetime-local',
        required: true,
      },
      { key: 'status', label: 'Статус', type: 'text', required: true },
    ],
  },
  metricsUser: {
    key: 'metricsUser',
    title: 'Метрики пользователей',
    apiPath: '/api/MetricsUser',
    idField: 'idMetricsUser',
    tableColumns: [
      { key: 'age', label: 'Возраст' },
      { key: 'gender', label: 'Пол' },
      { key: 'height', label: 'Рост (см)' },
      { key: 'weight', label: 'Вес (кг)' },
      { key: 'goalId', label: 'Цель' },
    ],
    formFields: [
      { key: 'height', label: 'Рост (см)', type: 'number', step: '0.1', required: true },
      { key: 'weight', label: 'Вес (кг)', type: 'number', step: '0.1', required: true },
      { key: 'age', label: 'Возраст', type: 'number', required: true },
      { key: 'gender', label: 'Пол', type: 'text', required: true },
      {
        key: 'experienceLevel',
        label: 'Уровень опыта',
        type: 'text',
        emptyAsNull: true,
      },
      {
        key: 'activityLevel',
        label: 'Уровень активности',
        type: 'text',
        emptyAsNull: true,
      },
      { 
        key: 'goalId', 
        label: 'Цель', 
        type: 'select', 
        required: true, 
        selectOptions: { apiPath: '/api/Goal', labelField: 'typeGoal', valueField: 'idGoal' } 
      },
    ],
  },
  workout: {
    key: 'workout',
    title: 'Тренировки',
    apiPath: '/api/Workout',
    idField: 'idWorkout',
    tableColumns: [
      { key: 'name', label: 'Название' },
      { key: 'description', label: 'Описание' },
      { key: 'duration', label: 'Длительность (мин)' },
      { key: 'complexity', label: 'Сложность' },
      { key: 'typeWorkout', label: 'Тип' },
    ],
    formFields: [
      { key: 'name', label: 'Название', type: 'text', required: true },
      { key: 'description', label: 'Описание', type: 'textarea', required: true },
      { key: 'duration', label: 'Длительность (мин)', type: 'number', required: true },
      { 
        key: 'complexity', 
        label: 'Сложность', 
        type: 'select', 
        required: true,
        staticOptions: [
          { label: 'Легко', value: 'Легко' },
          { label: 'Среднее', value: 'Среднее' },
          { label: 'Сложно', value: 'Сложно' },
        ]
      },
      { 
        key: 'typeWorkout', 
        label: 'Тип тренировки', 
        type: 'select', 
        required: true,
        staticOptions: [
          { label: 'Силовая', value: 'Силовая' },
          { label: 'Гипертрофия', value: 'Гипертрофия' },
          { label: 'Кардио', value: 'Кардио' },
          { label: 'Функциональная', value: 'Функциональная' },
        ]
      },
    ],
  },
  exercise: {
    key: 'exercise',
    title: 'Упражнения',
    apiPath: '/api/Exercise',
    idField: 'idExercise',
    gifUploadApiPath: '/api/Exercise',
    tableColumns: [
      { key: 'name', label: 'Название' },
      { key: 'complexity', label: 'Сложность' },
      { key: 'pictureUrl', label: 'Картинка' },
      { key: 'videoUrl', label: 'GIF', type: 'gif' },
    ],
    formFields: [
      { key: 'name', label: 'Название', type: 'text', required: true },
      { key: 'description', label: 'Описание', type: 'textarea', emptyAsNull: true },
      {
        key: 'complexity',
        label: 'Сложность',
        type: 'select',
        emptyAsNull: true,
        staticOptions: [
          { label: 'Легко', value: 'Легко' },
          { label: 'Среднее', value: 'Среднее' },
          { label: 'Сложно', value: 'Сложно' },
        ]
      },
      { key: 'pictureUrl', label: 'Picture URL', type: 'text', required: true },
    ],
  },
  trainingExercise: {
    key: 'trainingExercise',
    title: 'Связка тренировка ↔ упражнение',
    apiPath: '/api/TrainingExercise',
    idField: 'idTrainingExercise',
    tableColumns: [
      { key: 'workoutId', label: 'Тренировка' },
      { key: 'exerciseId', label: 'Упражнение' },
      { key: 'executionOrder', label: 'Порядок' },
      { key: 'sets', label: 'Подходы' },
      { key: 'repetitions', label: 'Повторения' },
      { key: 'restTime', label: 'Отдых (сек)' },
    ],
    formFields: [
      { 
        key: 'workoutId', 
        label: 'Тренировка', 
        type: 'select', 
        required: true, 
        selectOptions: { apiPath: '/api/Workout', labelField: 'name', valueField: 'idWorkout' } 
      },
      { 
        key: 'exerciseId', 
        label: 'Упражнение', 
        type: 'select', 
        required: true, 
        selectOptions: { apiPath: '/api/Exercise', labelField: 'name', valueField: 'idExercise' } 
      },
      { key: 'executionOrder', label: 'Порядок', type: 'number', required: true },
      { key: 'repetitions', label: 'Повторения', type: 'number', required: true },
      { key: 'sets', label: 'Подходы', type: 'number', required: true },
      { key: 'restTime', label: 'Отдых (сек)', type: 'number', required: true },
    ],
  },
  groupsMuscle: {
    key: 'groupsMuscle',
    title: 'Группы мышц',
    apiPath: '/api/GroupsMuscle',
    idField: 'idGroupsMuscle',
    tableColumns: [{ key: 'name', label: 'Название группы' }],
    formFields: [{ key: 'name', label: 'Название', type: 'text', required: true }],
  },
  exerciseGroupsMuscle: {
    key: 'exerciseGroupsMuscle',
    title: 'Связка упражнение ↔ группа мышц',
    apiPath: '/api/ExerciseGroupsMuscle',
    idField: 'idExerciseGroupsMuscle',
    tableColumns: [
      { key: 'exerciseId', label: 'Упражнение' },
      { key: 'groupsMusclesId', label: 'Группа мышц' },
    ],
    formFields: [
      { 
        key: 'exerciseId', 
        label: 'Упражнение', 
        type: 'select', 
        required: true, 
        selectOptions: { apiPath: '/api/Exercise', labelField: 'name', valueField: 'idExercise' } 
      },
      {
        key: 'groupsMusclesId',
        label: 'Группа мышц',
        type: 'select',
        required: true,
        selectOptions: { apiPath: '/api/GroupsMuscle', labelField: 'name', valueField: 'idGroupsMuscle' }
      },
    ],
  },
  planWorkout: {
    key: 'planWorkout',
    title: 'Планы тренировок',
    apiPath: '/api/PlanWorkout',
    idField: 'idPlanWorkout',
    tableColumns: [
      { key: 'userId', label: 'Пользователь' },
      { key: 'workoutId', label: 'Тренировка' },
      { key: 'status', label: 'Статус' },
      { key: 'source', label: 'Источник' },
    ],
    formFields: [
      { 
        key: 'userId', 
        label: 'Пользователь', 
        type: 'select', 
        required: true, 
        selectOptions: { apiPath: '/api/User', labelField: 'login', valueField: 'idUser' } 
      },
      { 
        key: 'workoutId', 
        label: 'Тренировка', 
        type: 'select', 
        required: true, 
        selectOptions: { apiPath: '/api/Workout', labelField: 'name', valueField: 'idWorkout' } 
      },
      { key: 'status', label: 'Статус', type: 'text', required: true },
      { key: 'source', label: 'Источник', type: 'text', emptyAsNull: true },
    ],
  },
  workoutCalendarAdmin: {
    key: 'workoutCalendarAdmin',
    title: 'Календарь тренировок',
    apiPath: '/api/WorkoutCalendar/admin',
    idField: 'idWorkoutCalendar',
    tableColumns: [
      { key: 'userId', label: 'Пользователь' },
      { key: 'workoutId', label: 'Тренировка' },
      { key: 'status', label: 'Статус' },
      { key: 'date', label: 'Дата', type: 'datetime-local' },
    ],
    formFields: [
      { 
        key: 'userId', 
        label: 'Пользователь', 
        type: 'select', 
        required: true, 
        selectOptions: { apiPath: '/api/User', labelField: 'login', valueField: 'idUser' } 
      },
      { 
        key: 'workoutId', 
        label: 'Тренировка', 
        type: 'select', 
        required: true, 
        selectOptions: { apiPath: '/api/Workout', labelField: 'name', valueField: 'idWorkout' } 
      },
      { key: 'status', label: 'Статус', type: 'text', emptyAsNull: true },
      { key: 'date', label: 'Дата', type: 'datetime-local', required: true },
    ],
  },
  workoutUserAdmin: {
    key: 'workoutUserAdmin',
    title: 'Сессии тренировок пользователей',
    apiPath: '/api/WorkoutUser/admin',
    idField: 'idWorkoutUser',
    tableColumns: [
      { key: 'userId', label: 'Пользователь' },
      { key: 'workoutId', label: 'Тренировка' },
      { key: 'duration', label: 'Длительность (мин)' },
      { key: 'status', label: 'Статус' },
      { key: 'date', label: 'Дата', type: 'datetime-local' },
    ],
    formFields: [
      { 
        key: 'userId', 
        label: 'Пользователь', 
        type: 'select', 
        required: true, 
        selectOptions: { apiPath: '/api/User', labelField: 'login', valueField: 'idUser' } 
      },
      { 
        key: 'workoutId', 
        label: 'Тренировка', 
        type: 'select', 
        required: true, 
        selectOptions: { apiPath: '/api/Workout', labelField: 'name', valueField: 'idWorkout' } 
      },
      { key: 'duration', label: 'Длительность (мин)', type: 'number', required: true },
      { key: 'status', label: 'Статус', type: 'text', emptyAsNull: true },
      { key: 'date', label: 'Дата', type: 'datetime-local', required: true },
    ],
  },
  workoutUserLogAdmin: {
    key: 'workoutUserLogAdmin',
    title: 'Логи тренировок',
    apiPath: '/api/WorkoutUserLog/admin',
    idField: 'idWorkoutUserLog',
    tableColumns: [
      { key: 'workoutUserId', label: 'Сессия' },
      { key: 'workoutId', label: 'Тренировка' },
      { key: 'repetitions', label: 'Повторения' },
      { key: 'sets', label: 'Подходы' },
      { key: 'weight', label: 'Вес' },
    ],
    formFields: [
      {
        key: 'workoutUserId',
        label: 'Сессия',
        type: 'number',
        required: true
      },
      { 
        key: 'workoutId', 
        label: 'Тренировка', 
        type: 'select', 
        required: true, 
        selectOptions: { apiPath: '/api/Workout', labelField: 'name', valueField: 'idWorkout' } 
      },
      { key: 'repetitions', label: 'Повторения', type: 'number', required: true },
      { key: 'sets', label: 'Подходы', type: 'number', required: true },
      { key: 'weight', label: 'Вес', type: 'number', step: '0.1', required: true },
      { key: 'status', label: 'Статус', type: 'text', emptyAsNull: true },
    ],
  },
  applicationLog: {
    key: 'applicationLog',
    title: 'Логи приложения',
    apiPath: '/api/ApplicationLog/admin',
    idField: 'idApplicationLog',
    tableColumns: [
      { key: 'createdAt', label: 'Время', type: 'datetime-local' },
      { key: 'action', label: 'Действие' },
      { key: 'entityType', label: 'Тип сущности' },
      { key: 'entityId', label: 'ID сущности' },
      { key: 'userId', label: 'ID пользователя' },
      { key: 'status', label: 'Статус' },
      { key: 'errorMessage', label: 'Ошибка' },
    ],
    formFields: [],
  },
}
