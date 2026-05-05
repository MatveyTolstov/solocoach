# Таблица классов проекта SoloCoachApi2

## Бэкенд (SoloCoachApi — ASP.NET Core)

| №  | Название                    | Тип класса           | Описание |
|----|-----------------------------|----------------------|----------|
| 1  | User                        | entity (Model)       | Сущность пользователя: IdUser, Name, Login, Password, Email, RoleId, MetricsUserId |
| 2  | Role                        | entity (Model)       | Сущность роли пользователя: IdRole, RoleName |
| 3  | MetricsUser                 | entity (Model)       | Физические параметры пользователя: Height, Weight, Age, Gender, ExperienceLevel, ActivityLevel, GoalId |
| 4  | Goal                        | entity (Model)       | Цель тренировок: TypeGoal, TargetWeight, DateStart, DateEnd, Status |
| 5  | Exercise                    | entity (Model)       | Упражнение: Name, Description, Complexity, PictureUrl |
| 6  | GroupsMuscle                | entity (Model)       | Группа мышц: IdGroupsMuscle, Name |
| 7  | ExerciseGroupsMuscle        | entity (Model)       | Связь упражнение–группа мышц (M:M): ExerciseId, GroupsMusclesId |
| 8  | Workout                     | entity (Model)       | Тренировка: Name, Description, Duration, Complexity, TypeWorkout |
| 9  | TrainingExercise            | entity (Model)       | Упражнение внутри тренировки: WorkoutId, ExerciseId, ExecutionOrder, Repetitions, Sets, RestTime |
| 10 | WorkoutUser                 | entity (Model)       | Выполненная тренировка пользователем: UserId, WorkoutId, Duration, Date, Status |
| 11 | WorkoutUserLog              | entity (Model)       | Лог упражнений сессии: WorkoutUserId, WorkoutId, Repetitions, Sets, Weight, Status |
| 12 | WorkoutCalendar             | entity (Model)       | Запланированная тренировка в календаре: UserId, WorkoutId, Date, Status |
| 13 | PlanWorkout                 | entity (Model)       | План тренировки для пользователя: UserId, WorkoutId, Status, Source |
| 14 | ApplicationLog              | entity (Model)       | Журнал действий в системе: Action, EntityType, EntityId, UserId, Details (JSON), CreatedAt, Status, ErrorMessage |
| 15 | PasswordResetToken          | entity (Model)       | Токен сброса пароля: UserId, TokenHash, ExpiresAt, IsUsed |
| 16 | ApplicationContext          | class (DbContext)    | EF Core контекст БД (PostgreSQL) — содержит DbSet для всех сущностей; управляет транзакциями |
| 17 | IAuthService                | interface            | Контракт сервиса аутентификации: LoginAsync, RegisterAsync, ForgotPasswordAsync, ResetPasswordAsync |
| 18 | IEmailService               | interface            | Контракт почтового сервиса: SendEmailAsync |
| 19 | ILoggingService             | interface            | Контракт сервиса логирования: LogActionAsync |
| 20 | IUserRepository             | interface            | Контракт репозитория пользователей: CRUD-операции |
| 21 | IWorkoutRepository          | interface            | Контракт репозитория тренировок: CRUD + пагинация |
| 22 | AuthService                 | class (Service)      | Реализует IAuthService; JWT-аутентификация, регистрация, сброс пароля через email |
| 23 | JwtService                  | class (Service)      | Генерация и валидация JWT-токенов |
| 24 | SmtpEmailService            | class (Service)      | Реализует IEmailService; отправка писем через SMTP |
| 25 | UserService                 | class (Service)      | CRUD пользователей, смена email с кодом подтверждения |
| 26 | WorkoutService              | class (Service)      | CRUD тренировок с поддержкой пагинации |
| 27 | ExerciseService             | class (Service)      | CRUD упражнений с поддержкой пагинации |
| 28 | WorkoutSessionService       | class (Service)      | Завершение тренировочной сессии: сохраняет WorkoutUser, WorkoutUserLog, запись в журнал |
| 29 | AiService                   | class (Service)      | Генерация персональных рекомендаций тренировок на базе профиля пользователя |
| 30 | ApplicationLogService       | class (Service)      | Реализует ILoggingService; запись и чтение журнала системных действий |
| 31 | ProfileService              | class (Service)      | Получение и обновление расширенного профиля пользователя |
| 32 | AuthController              | class (Controller)   | POST /api/auth — login, register, forgot-password, reset-password |
| 33 | UserController              | class (Controller)   | CRUD /api/user — управление пользователями; запрос и подтверждение смены email |
| 34 | WorkoutController           | class (Controller)   | CRUD /api/workout — управление тренировками, пагинация |
| 35 | ExerciseController          | class (Controller)   | CRUD /api/exercise — управление упражнениями, пагинация |
| 36 | WorkoutSessionController    | class (Controller)   | POST /api/workoutsession/complete — завершение тренировочной сессии |
| 37 | AiController                | class (Controller)   | GET /api/ai/recommend — персональные рекомендации (Authorize) |
| 38 | ApplicationLogController    | class (Controller)   | GET /api/applicationlog — чтение журнала действий с фильтрацией (Authorize) |
| 39 | BackupController            | class (Controller)   | POST /api/backup/create — создание резервной копии PostgreSQL через pg_dump |

---

## Фронтенд (solocoachsite — React SPA)

| №  | Название                    | Тип класса           | Описание |
|----|-----------------------------|----------------------|----------|
| 40 | AuthSession                 | interface            | Декодированные данные JWT: userId, login, roleId |
| 41 | AccessRoles                 | interface            | Флаги доступа пользователя: admin, manager (boolean) |
| 42 | ApiOptions                  | interface            | Параметры запроса к API: token, method, query, body |
| 43 | EntityConfig                | interface            | Конфигурация сущности для CrudSection: key, title, apiPath, idField, tableColumns, formFields |
| 44 | Toast                       | interface            | Всплывающее уведомление: id, message, type |
| 45 | SnapshotData                | interface            | Аналитический снапшот: usersTotal, avgAge, roleDistribution, ageBuckets, genderDistribution и др. |
| 46 | AuthUtils                   | utility (module)     | Утилиты работы с токеном: readStoredToken, writeStoredToken, parseJwtSession, resolveRoleAccess |
| 47 | ApiService                  | utility (module)     | Универсальный HTTP-клиент: apiRequest — строит URL, добавляет заголовок Authorization Bearer |
| 48 | Helpers                     | utility (module)     | Вспомогательные функции: ensureArray, formatDateTime, countBy, toNullableNumber |
| 49 | ToastProvider               | class (Context)      | React-контекст уведомлений: notify(message, type), dismiss(id); автоскрытие через 4.5 с |
| 50 | App                         | component            | Корневой компонент: JWT-аутентификация, маршрутизация workspace (manager/admin), управление вкладками |
| 51 | LoginScreen                 | component            | Форма входа: поля login/password, вызов onLogin, отображение ошибок и состояния загрузки |
| 52 | ManagerWorkspace            | component            | Контейнер рабочего пространства менеджера: AnalyticsPanel или CrudSection по activeTab |
| 53 | AdminWorkspace              | component            | Контейнер рабочего пространства администратора: LogsPanel, UserManagementSection, CrudSection, BackupSection |
| 54 | AnalyticsPanel              | component            | Аналитическая панель: загружает агрегированную статистику, отображает StatCard + 4 графика |
| 55 | CrudSection                 | component            | Универсальный CRUD-раздел: таблица с поиском, inline-редактирование, создание/удаление записей через API |
| 56 | LogsPanel                   | component            | Просмотр журнала действий ApplicationLog с поиском по полям |
| 57 | UserManagementSection       | component            | Управление пользователями: CRUD с назначением ролей, inline-редактирование |
| 58 | StatCard                    | component            | Карточка KPI-метрики: label + value |
| 59 | RoleDistributionChart       | component            | Диаграмма (Doughnut) распределения пользователей по ролям |
| 60 | AgeDistributionChart        | component            | Гистограмма (Bar) распределения пользователей по возрастным группам |
| 61 | GenderDistributionChart     | component            | Круговая диаграмма (Pie) распределения пользователей по полу |
| 62 | ActivityDistributionChart   | component            | Гистограмма (Bar) распределения пользователей по уровню активности |

---

## Мобильное приложение (solocouchmobile — Flutter, Clean Architecture)

### Core

| №   | Название                          | Тип класса              | Описание |
|-----|-----------------------------------|-------------------------|----------|
| 63  | Failure                           | abstract class          | Базовый класс ошибок (extends Equatable): message (String?) |
| 64  | ServerFailure                     | class (Failure)         | Ошибка сервера |
| 65  | NetworkFailure                    | class (Failure)         | Сетевая ошибка |
| 66  | AuthFailure                       | class (Failure)         | Ошибка аутентификации |
| 67  | CacheFailure                      | class (Failure)         | Ошибка кэша |
| 68  | AppLogger                         | class (utility)         | Статическое логирование: v(), d(), i(), w(), e() |
| 69  | AppBlocObserver                   | class (BlocObserver)    | Наблюдатель BLoC: onCreate, onClose, onChange, onError |
| 70  | DioClient                         | class (utility)         | HTTP-клиент Dio: хранит токен в SharedPreferences, перехватывает 401, mapDioError() |
| 71  | LoggingInterceptor                | class (Interceptor)     | Логирует запросы/ответы/ошибки Dio |
| 72  | ApiConfig                         | class (constants)       | Константа baseUrl |
| 73  | WorkoutCalendarStatus             | abstract class          | Константы статусов календаря: planned, completed |
| 74  | WorkoutFilterOptions              | abstract class          | Списки фильтров тренировок: typeWorkouts, complexities |
| 75  | ExerciseFilterOptions             | abstract class          | Список фильтров сложности упражнений |

### Domain — Entities

| №   | Название                          | Тип класса              | Описание |
|-----|-----------------------------------|-------------------------|----------|
| 76  | GoalEntity                        | entity (Domain)         | Цель пользователя: idGoal, typeGoal, targetWeight, dateStart, dateEnd, status |
| 77  | ProfileEntity                     | entity (Domain)         | Профиль: idUser, name, login, email, metricsUserId, metrics, currentGoal |
| 78  | MetricsUserEntity                 | entity (Domain)         | Метрики: height, weight, age, gender, experienceLevel, activityLevel, goalId |
| 79  | ExerciseEntity                    | entity (Domain)         | Упражнение: idExercise, name, description, complexity, pictureUrl |
| 80  | WorkoutEntity                     | entity (Domain)         | Тренировка: idWorkout, name, description, duration, complexity, typeWorkout, trainingExercises |
| 81  | WorkoutTrainingExerciseEntity     | entity (Domain)         | Упражнение в тренировке: idTrainingExercise, executionOrder, repetitions, sets, restTime, exercise |
| 82  | WorkoutCalendarEntity             | entity (Domain)         | Запись календаря: idWorkoutCalendar, userId, workoutId, date, status |
| 83  | PagedWorkoutsResult               | entity (Domain)         | Страничный результат тренировок: items, totalCount, page, pageSize, hasMore |
| 84  | PagedExercisesResult              | entity (Domain)         | Страничный результат упражнений: items, totalCount, page, pageSize, hasMore |
| 85  | AiWorkoutRecommendationEntity     | entity (Domain)         | AI-рекомендация: workout, reason |
| 86  | AiRecommendationResponseEntity    | entity (Domain)         | Ответ AI: recommendations, advice, disclaimer |

### Domain — Repositories (interfaces)

| №   | Название                          | Тип класса              | Описание |
|-----|-----------------------------------|-------------------------|----------|
| 87  | AuthRepository                    | abstract class          | Контракт: login, register, forgotPassword, resetPassword, logout, hasSession, getToken |
| 88  | ProfileRepository                 | abstract class          | Контракт: getProfile, getGoals, updateProfile |
| 89  | WorkoutRepository                 | abstract class          | Контракт: getAll, getPage, getById |
| 90  | ExerciseRepository                | abstract class          | Контракт: getAll, getPage, getById |
| 91  | WorkoutCalendarRepository         | abstract class          | Контракт: getMine, create, update, delete |
| 92  | WorkoutSessionRepository          | abstract class          | Контракт: complete |
| 93  | AiRepository                      | abstract class          | Контракт: getRecommendations |

### Domain — Use Cases

| №   | Название                          | Тип класса              | Описание |
|-----|-----------------------------------|-------------------------|----------|
| 94  | LoginUseCase                      | class (UseCase)         | Вход пользователя |
| 95  | RegisterUseCase                   | class (UseCase)         | Регистрация пользователя |
| 96  | ForgotPasswordUseCase             | class (UseCase)         | Запрос сброса пароля по email |
| 97  | ResetPasswordUseCase              | class (UseCase)         | Сброс пароля по коду |
| 98  | GetProfileUseCase                 | class (UseCase)         | Получение профиля текущего пользователя |
| 99  | GetGoalsUseCase                   | class (UseCase)         | Получение списка целей пользователя |
| 100 | UpdateProfileUseCase              | class (UseCase)         | Обновление профиля и метрик |
| 101 | GetWorkoutsUseCase                | class (UseCase)         | Получение всех тренировок |
| 102 | GetWorkoutsPageUseCase            | class (UseCase)         | Постраничное получение тренировок с фильтрацией |
| 103 | GetWorkoutByIdUseCase             | class (UseCase)         | Получение тренировки по ID |
| 104 | CompleteWorkoutSessionUseCase     | class (UseCase)         | Завершение тренировочной сессии |
| 105 | GetExercisesUseCase               | class (UseCase)         | Получение всех упражнений |
| 106 | GetExercisesPageUseCase           | class (UseCase)         | Постраничное получение упражнений с фильтрацией |
| 107 | GetExerciseByIdUseCase            | class (UseCase)         | Получение упражнения по ID |
| 108 | GetAiRecommendationsUseCase       | class (UseCase)         | Получение AI-рекомендаций тренировок |

### Data — Models

| №   | Название                          | Тип класса              | Описание |
|-----|-----------------------------------|-------------------------|----------|
| 109 | GoalModel                         | class (Model)           | JSON-модель цели: fromJson, toEntity |
| 110 | ProfileModel                      | class (Model)           | JSON-модель профиля: fromJson, toEntity |
| 111 | MetricsUserModel                  | class (Model)           | JSON-модель метрик: fromJson, toEntity |
| 112 | ExerciseModel                     | class (Model)           | JSON-модель упражнения: fromJson, toEntity |
| 113 | WorkoutModel                      | class (Model)           | JSON-модель тренировки: fromJson, toEntity |
| 114 | WorkoutTrainingExerciseModel      | class (Model)           | JSON-модель упражнения в тренировке: fromJson, toEntity |
| 115 | WorkoutCalendarModel              | class (Model)           | JSON-модель записи календаря: fromJson, toJson, toEntity |
| 116 | PagedWorkoutsResultModel          | class (Model)           | JSON-модель страничного результата тренировок: fromJson |
| 117 | PagedExercisesResultModel         | class (Model)           | JSON-модель страничного результата упражнений: fromJson |
| 118 | AiWorkoutRecommendationModel      | class (Model)           | JSON-модель AI-рекомендации: fromJson |
| 119 | AiRecommendationResponseModel     | class (Model)           | JSON-модель ответа AI: fromJson |

### Data — DataSources

| №   | Название                          | Тип класса              | Описание |
|-----|-----------------------------------|-------------------------|----------|
| 120 | AuthRemoteDataSource              | abstract class          | Контракт: login, register, forgotPassword, resetPassword |
| 121 | AuthRemoteDataSourceImpl          | class (DataSource)      | Реализация через Dio |
| 122 | ProfileRemoteDataSource           | abstract class          | Контракт: getMe, updateMe, getGoals |
| 123 | ProfileRemoteDataSourceImpl       | class (DataSource)      | Реализация через Dio |
| 124 | WorkoutRemoteDataSource           | abstract class          | Контракт: getAll, getPaged, getById |
| 125 | WorkoutRemoteDataSourceImpl       | class (DataSource)      | Реализация через Dio |
| 126 | ExerciseRemoteDataSource          | abstract class          | Контракт: getAll, getPaged, getById |
| 127 | ExerciseRemoteDataSourceImpl      | class (DataSource)      | Реализация через Dio |
| 128 | WorkoutCalendarRemoteDataSource   | abstract class          | Контракт: getMine, create, update, delete |
| 129 | WorkoutCalendarRemoteDataSourceImpl | class (DataSource)    | Реализация через Dio |
| 130 | WorkoutSessionRemoteDataSource    | abstract class          | Контракт: complete |
| 131 | WorkoutSessionRemoteDataSourceImpl | class (DataSource)     | Реализация через Dio |
| 132 | AiRemoteDataSource                | abstract class          | Контракт: getRecommendations |
| 133 | AiRemoteDataSourceImpl            | class (DataSource)      | Реализация через Dio |

### Data — Repository Implementations

| №   | Название                          | Тип класса              | Описание |
|-----|-----------------------------------|-------------------------|----------|
| 134 | AuthRepositoryImpl                | class (Repository)      | Реализует AuthRepository; хранит токен в SharedPreferences |
| 135 | ProfileRepositoryImpl             | class (Repository)      | Реализует ProfileRepository |
| 136 | WorkoutRepositoryImpl             | class (Repository)      | Реализует WorkoutRepository |
| 137 | ExerciseRepositoryImpl            | class (Repository)      | Реализует ExerciseRepository |
| 138 | WorkoutCalendarRepositoryImpl     | class (Repository)      | Реализует WorkoutCalendarRepository |
| 139 | WorkoutSessionRepositoryImpl      | class (Repository)      | Реализует WorkoutSessionRepository |
| 140 | AiRepositoryImpl                  | class (Repository)      | Реализует AiRepository |

### Presentation — BLoC / Cubit

| №   | Название                          | Тип класса              | Описание |
|-----|-----------------------------------|-------------------------|----------|
| 141 | AuthBloc                          | class (Bloc)            | Аутентификация: события AuthCheckRequested, AuthLoginSubmitted, AuthRegisterSubmitted, AuthLogoutRequested |
| 142 | ForgotPasswordBloc                | class (Bloc)            | Запрос сброса пароля: ForgotPasswordSubmitted → Loading/Success/Failure |
| 143 | ResetPasswordBloc                 | class (Bloc)            | Сброс пароля по коду: ResetPasswordSubmitted → Loading/Success/Failure |
| 144 | ProfileCubit                      | class (Cubit)           | Профиль пользователя: load(), save(); состояния Initial/Loading/Ready/Error |
| 145 | WorkoutsCubit                     | class (Cubit)           | Список тренировок с пагинацией и фильтрацией: load, refresh, loadMore, applyFilters |
| 146 | WorkoutDetailCubit                | class (Cubit)           | Детали тренировки: load(id) |
| 147 | ExercisesCubit                    | class (Cubit)           | Список упражнений с пагинацией и фильтрацией: load, refresh, loadMore, applyFilters |
| 148 | ExerciseDetailCubit               | class (Cubit)           | Детали упражнения: load(id) |
| 149 | TrainingCalendarCubit             | class (Cubit)           | Календарь тренировок: load, scheduleWorkout, setEntryCompleted, deleteEntry |
| 150 | AiRecommendationsCubit            | class (Cubit)           | AI-рекомендации: load() |

### Presentation — Pages / Widgets

| №   | Название                          | Тип класса              | Описание |
|-----|-----------------------------------|-------------------------|----------|
| 151 | SoloCoachApp                      | widget (StatefulWidget) | Корневой виджет приложения с темой и DI |
| 152 | AuthGate                          | widget (StatelessWidget)| Маршрутизатор: перенаправляет на HomeShellPage или LoginPage по состоянию AuthBloc |
| 153 | LoginPage                         | widget (StatefulWidget) | Форма входа: login/password, ошибки, состояние загрузки |
| 154 | RegisterPage                      | widget (StatefulWidget) | Форма регистрации: name, login, email, password |
| 155 | ForgotPasswordPage                | widget (StatefulWidget) | Ввод email для сброса пароля |
| 156 | ResetPasswordPage                 | widget (StatefulWidget) | Ввод кода подтверждения и нового пароля |
| 157 | HomeShellPage                     | widget (StatefulWidget) | Главная оболочка: NavigationBar с вкладками (тренировки, упражнения, календарь, AI, профиль) |
| 158 | ProfilePage                       | widget (StatelessWidget)| Профиль: карточка пользователя, вес/цель, редактирование метрик, диалог достижения цели с конфетти |
| 159 | AccountCredentialsPage            | widget (StatefulWidget) | Смена логина и пароля |
| 160 | ChangeEmailPage                   | widget (StatefulWidget) | Запрос смены email через API |
| 161 | WorkoutsPage                      | widget (StatelessWidget)| Каталог тренировок: поиск, фильтры, бесконечная прокрутка, карточки |
| 162 | WorkoutDetailPage                 | widget (StatelessWidget)| Детали тренировки: описание, список упражнений, кнопка начала сессии |
| 163 | WorkoutSessionPage                | widget (StatefulWidget) | Активная тренировочная сессия: ввод повторений/подходов/веса, таймер отдыха, завершение |
| 164 | ExercisesPage                     | widget (StatelessWidget)| Каталог упражнений: поиск, фильтр сложности, бесконечная прокрутка, карточки |
| 165 | ExerciseDetailPage                | widget (StatelessWidget)| Детали упражнения: изображение, описание, сложность |
| 166 | TrainingCalendarPage              | widget (StatelessWidget)| Календарь тренировок: сетка месяца, плитки записей, планирование и отметка выполнения |
| 167 | AiRecommendationsPage             | widget (StatelessWidget)| AI-рекомендации: начальный экран, загрузка, карточки рекомендаций, карточка совета |
