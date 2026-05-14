# СамСебеТренер — SoloCoach

Платформа для персональных фитнес-тренировок, состоящая из трёх компонентов:

- **SoloCoachApi** — REST API на ASP.NET Core 8.0
- **solocoachsite** — административная панель на React/Vite
- **SoloCoachMobile** — мобильное приложение на Flutter 3.27 (отдельный репозиторий)

---

## Технологический стек

| Слой | Технология |
|---|---|
| Бэкенд | ASP.NET Core 8.0, EF Core 8, PostgreSQL 16 |
| Аутентификация | JWT Bearer, BCrypt.Net |
| Хранилище файлов | AWS S3 (медиафайлы упражнений) |
| Почта | SMTP / MailKit (подтверждение email) |
| ИИ | OpenRouter API (рекомендации тренировок) |
| Админ-панель | React 18, Vite |
| Мобильное приложение | Flutter 3.27, Dart, BLoC/Cubit |
| Развёртывание | Docker, docker-compose, nginx |

---

## Быстрый старт (Docker)

### 1. Создать файл `.env` в корне проекта

```env
SMTP_HOST=smtp.example.com
SMTP_PORT=587
SMTP_USER=your@email.com
SMTP_PASSWORD=your_smtp_password

S3_SERVICE_URL=https://s3.amazonaws.com
S3_BUCKET_NAME=solocoach-media
S3_REGION=us-east-1
S3_ACCESS_KEY=your_access_key
S3_SECRET_KEY=your_secret_key
S3_PUBLIC_URL=https://solocoach-media.s3.amazonaws.com

OPENROUTER_API_KEY=your_openrouter_key
OPENROUTER_MODEL=openai/gpt-4o-mini
```

### 2. Запустить все сервисы

```bash
docker-compose up --build -d
```

Запускаются следующие контейнеры:
- `solocoach_db` — PostgreSQL 16 во внутренней сети
- `solocoach_api` — API на ASP.NET Core
- `solocoach_site` — React-панель администратора
- `solocoach_nginx` — обратный прокси nginx на портах 80 / 443

База данных инициализируется из `init.sql` при первом запуске.

---

## Локальная разработка

### API (ASP.NET Core)

Требования: .NET 8 SDK, PostgreSQL 16

```bash
cd SoloCoachApi
```

Создать `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=SoloCoach;Username=postgres;Password=1;Port=5432"
  },
  "Smtp": {
    "Host": "smtp.example.com",
    "Port": 587,
    "User": "your@email.com",
    "Password": "your_smtp_password"
  },
  "S3": {
    "ServiceUrl": "https://s3.amazonaws.com",
    "BucketName": "solocoach-media",
    "Region": "us-east-1",
    "AccessKey": "your_access_key",
    "SecretKey": "your_secret_key",
    "PublicUrl": "https://solocoach-media.s3.amazonaws.com"
  },
  "OpenRouter": {
    "ApiKey": "your_openrouter_key",
    "Model": "openai/gpt-4o-mini"
  }
}
```

Применить миграции и запустить:

```bash
dotnet ef database update
dotnet run
```

API доступен по адресу `http://localhost:5000`. Swagger UI — `http://localhost:5000/swagger`.

### Административная панель (React/Vite)

Требования: Node.js 20+

```bash
cd solocoachsite
npm install
npm run dev
```

Панель доступна по адресу `http://localhost:5173`.

### Мобильное приложение (Flutter)

Требования: Flutter 3.27, Dart SDK

```bash
cd SoloCoachMobile/solocouchmobile
flutter pub get
flutter run
```

---

## Роли пользователей

В системе предусмотрено три роли:

| ID | Название | Описание |
|---|---|---|
| 1 | Admin | Администратор. Имеет доступ к административной панели: управление пользователями, просмотр и очистка журнала аудита, резервное копирование БД. Не имеет доступа к менеджерскому кабинету. |
| 2 | User | Обычный пользователь. Работает только через мобильное приложение. Нет доступа к веб-панели. |
| 3 | Manager | Менеджер. Имеет доступ к менеджерскому кабинету: аналитика, управление каталогом тренировок и упражнений. Нет доступа к административному разделу. |

Роль хранится в JWT-токене и проверяется как на стороне API (`[Authorize(Roles = "1")]`), так и на стороне веб-панели при маршрутизации по кабинетам.

---

## API-эндпоинты

Полная интерактивная документация доступна по адресу `/swagger` при запущенном API.

| Группа | Префикс | Описание |
|---|---|---|
| Аутентификация | `/api/Auth` | Регистрация, вход, подтверждение email |
| Профиль | `/api/Profile` | Профиль пользователя, метрики, цели |
| Пользователи | `/api/User` | Управление пользователями (администратор) |
| Тренировки | `/api/Workout` | Каталог тренировок |
| Упражнения | `/api/Exercise` | Каталог упражнений |
| Упражнения в тренировке | `/api/TrainingExercise` | Упражнения внутри тренировок |
| Календарь | `/api/WorkoutCalendar` | Запланированные тренировки |
| Сессии | `/api/WorkoutSession` | Активные тренировочные сессии |
| Логи сессий | `/api/WorkoutUserLog` | Логи упражнений за сессию |
| ИИ | `/api/Ai` | ИИ-рекомендации тренировок |
| Группы мышц | `/api/GroupsMuscle` | Группы мышц |
| Роли | `/api/Role` | Роли пользователей (администратор) |
| Логи приложения | `/api/ApplicationLog` | Журнал аудита (администратор) |
| Резервные копии | `/api/Backup` | Резервное копирование БД (администратор) |

Все эндпоинты, кроме `/api/Auth/register` и `/api/Auth/login`, требуют заголовка `Authorization: Bearer <токен>`.

---

## Структура проекта

```
SoloCouchApi2/
├── SoloCoachApi/          # ASP.NET Core API
│   ├── Controllers/
│   ├── Services/
│   ├── Repositories/
│   ├── Models/
│   ├── DTOs/
│   ├── Mappers/
│   ├── DataBase/          # EF Core DbContext, миграции
│   └── Dockerfile
├── solocoachsite/         # React-панель администратора
│   ├── src/
│   │   ├── components/
│   │   ├── pages/
│   │   └── utils/
│   └── Dockerfile
├── nginx/
│   └── default.conf
├── init.sql               # Начальная схема БД
├── docker-compose.yml
└── .env                   # Секреты (не коммитится)
```

---

## Справочник переменных окружения

| Переменная | Описание |
|---|---|
| `SMTP_HOST` | Хост SMTP-сервера |
| `SMTP_PORT` | Порт SMTP-сервера (обычно 587) |
| `SMTP_USER` | Логин SMTP |
| `SMTP_PASSWORD` | Пароль SMTP |
| `S3_SERVICE_URL` | URL S3-совместимого хранилища |
| `S3_BUCKET_NAME` | Имя S3-бакета |
| `S3_REGION` | Регион S3 |
| `S3_ACCESS_KEY` | Ключ доступа S3 |
| `S3_SECRET_KEY` | Секретный ключ S3 |
| `S3_PUBLIC_URL` | Публичный базовый URL медиафайлов |
| `OPENROUTER_API_KEY` | API-ключ OpenRouter |
| `OPENROUTER_MODEL` | ID модели, например `openai/gpt-4o-mini` |
