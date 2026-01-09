# MusicWeb API

REST API сервис для управления музыкальной библиотекой (артисты, песни, жанры).

## Технологии

- **.NET 8** — платформа
- **PostgreSQL** — база данных
- **Redis** — кэширование
- **EF Core + Dapper** — ORM и микро-ORM
- **JWT + API Key** — аутентификация
- **Liquibase** — миграции БД
- **Prometheus + Grafana** — метрики
- **Docker Compose** — контейнеризация
- **xUnit** — тестирование

## Реализованные функции

### Архитектура
- Слоёная архитектура (Controllers, Services, Repositories)
- DTO для запросов и ответов
- Бизнес-логика вынесена в сервисы
- Асинхронность (async/await + CancellationToken)

### База данных
- PostgreSQL через Docker Compose
- Миграции через Liquibase
- Many-to-Many связь (Song - Genre через SongGenre)

### ORM
- CRUD через EF Core (Songs, Genres)
- CRUD через Dapper (Artists)
- Транзакции в Dapper (создание артиста с песнями)

### Безопасность
- JWT Bearer аутентификация
- API Key аутентификация
- Ролевой доступ (Admin, Manager, User)
- Матрица авторизации (политики доступа)

### Кэширование
- Redis для GET-запросов
- Инвалидация кэша при CUD операциях

### Мониторинг
- Prometheus метрики (/metrics)
- Health Checks (PostgreSQL, Redis)
- Структурированное логирование
- Request logging middleware

### API
- Swagger UI с документацией
- ProducesResponseType для всех эндпоинтов
- Пагинация и фильтрация
- Централизованная обработка ошибок
- Единый формат ответа об ошибках

### Тестирование
- Unit-тесты репозиториев
- Testcontainers для PostgreSQL

## Быстрый старт

### Требования
- Docker и Docker Compose
- .NET 8 SDK

### Запуск

```bash
# Клонировать репозиторий
git clone <repository-url>
cd MusicWeb

# Запустить все сервисы
docker-compose up -d

# Проверить статус
docker-compose ps
```

### Сервисы

| Сервис | URL | Описание |
|--------|-----|----------|
| API | http://localhost:5000 | REST API |
| Swagger | http://localhost:5000/swagger | Документация API |
| Prometheus | http://localhost:9090 | Метрики |
| Grafana | http://localhost:3000 | Дашборды |
| PostgreSQL | localhost:5432 | База данных |
| Redis | localhost:6379 | Кэш |

## API Эндпоинты

### Аутентификация

```http
POST /api/auth/register    # Регистрация пользователя
POST /api/auth/login       # Получение JWT токена
GET  /api/auth/me          # Текущий пользователь [Authorize]
```

### Артисты

```http
GET    /api/artists              # Список артистов
GET    /api/artists/{id}         # Артист по ID
POST   /api/artists              # Создать артиста [Manager+]
POST   /api/artists/with-songs   # Создать артиста с песнями [Manager+]
PUT    /api/artists/{id}         # Обновить артиста [Manager+]
DELETE /api/artists/{id}         # Удалить артиста [Admin]
```

### Песни

```http
GET    /api/songs/all            # Все песни
GET    /api/songs                # Песни с пагинацией и фильтрацией
GET    /api/songs/{id}           # Песня по ID
POST   /api/songs                # Создать песню [Manager+]
PUT    /api/songs/{id}           # Обновить песню [Manager+]
PUT    /api/songs/{id}/genres    # Установить жанры песни [Manager+]
DELETE /api/songs/{id}           # Удалить песню [Manager+]
```

### Жанры

```http
GET    /api/genres               # Список жанров
GET    /api/genres/{id}          # Жанр по ID
POST   /api/genres               # Создать жанр [Manager+]
PUT    /api/genres/{id}          # Обновить жанр [Manager+]
DELETE /api/genres/{id}          # Удалить жанр [Admin]
```

### Пользователи (только Admin)

```http
GET    /api/user                 # Список пользователей
GET    /api/user/{id}            # Пользователь по ID
POST   /api/user                 # Создать пользователя
PUT    /api/user/{id}            # Обновить пользователя
PATCH  /api/user/{id}/role       # Изменить роль
PATCH  /api/user/{id}/password   # Изменить пароль
DELETE /api/user/{id}            # Удалить пользователя
```

### Health Checks

```http
GET /api/health          # Полная проверка (PostgreSQL + Redis)
```

### Метрики

```http
GET /metrics             # Prometheus метрики
```

## Аутентификация

### JWT Bearer

```bash
# 1. Получить токен
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username": "admin", "password": "admin"}'

# Ответ:
# {"accessToken": "eyJhbG...", "expiresInSeconds": 3600, "role": "Admin"}

# 2. Использовать токен
curl http://localhost:5000/api/artists \
  -H "Authorization: Bearer eyJhbG..."
```

### API Key

```bash
curl http://localhost:5000/api/health \
  -H "X-Api-Key: your-api-key"
```

API ключи настраиваются в `appsettings.json`:

```json
{
  "ApiKeys": {
    "your-api-key": "service-name"
  }
}
```

## Роли и права доступа

| Действие | User | Manager | Admin |
|----------|------|---------|-------|
| Просмотр контента | + | + | + |
| Создание/редактирование контента | - | + | + |
| Удаление контента | - | - | + |
| Управление пользователями | - | - | + |

## Фильтрация и пагинация

```http
GET /api/songs?page=1&pageSize=10&search=love&artistId=1&genreId=2&sortBy=title&sortDirection=desc
```

| Параметр | Описание | По умолчанию |
|----------|----------|--------------|
| page | Номер страницы | 1 |
| pageSize | Размер страницы (1-100) | 10 |
| search | Поиск по названию, тексту, артисту | - |
| artistId | Фильтр по артисту | - |
| genreId | Фильтр по жанру | - |
| sortBy | Сортировка: id, title, artist | id |
| sortDirection | Направление: asc, desc | asc |

Ответ:

```json
{
  "items": [...],
  "total": 100,
  "page": 1,
  "pageSize": 10,
  "totalPages": 10,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

## Формат ошибок

Все ошибки возвращаются в едином формате:

```json
{
  "error": "NotFound",
  "message": "Artist with id '999' was not found.",
  "traceId": "00-abc123...",
  "validationErrors": null
}
```

Ошибки валидации:

```json
{
  "error": "ValidationError",
  "message": "One or more validation errors occurred.",
  "traceId": "00-abc123...",
  "validationErrors": {
    "Name": ["'Name' must not be empty."],
    "ArtistId": ["'Artist Id' must be greater than '0'."]
  }
}
```

## Конфигурация

### appsettings.json

```json
{
  "ConnectionStrings": {
    "Postgres": "Host=localhost;Database=music;Username=music;Password=music"
  },
  "Redis": {
    "ConnectionString": "localhost:6379"
  },
  "Jwt": {
    "Key": "your-secret-key-min-32-characters-long",
    "Issuer": "MusicWeb",
    "Audience": "MusicWeb",
    "AccessTokenLifetimeSeconds": 3600
  },
  "ApiKeys": {
    "service-key-1": "monitoring-service",
    "service-key-2": "external-service"
  }
}
```

### Переменные окружения

```bash
ConnectionStrings__Postgres=Host=db;Database=music;Username=music;Password=music
Redis__ConnectionString=redis:6379
Jwt__Key=your-secret-key
```

## Структура проекта

```
MusicWeb/
├── src/
│   ├── Controllers/          # API контроллеры
│   ├── Services/             # Бизнес-логика
│   │   ├── Artist/
│   │   ├── Songs/
│   │   ├── Genres/
│   │   ├── Auth/
│   │   ├── Users/
│   │   └── Caching/
│   ├── Repositories/         # Доступ к данным
│   ├── Models/
│   │   ├── Entities/         # Сущности БД
│   │   ├── Dtos/             # DTO объекты
│   │   └── Enums/
│   ├── Data/                 # DbContext
│   ├── Middleware/           # Middleware
│   ├── Auth/                 # Авторизация
│   ├── Validation/           # FluentValidation
│   ├── Mapping/              # Маппинг Entity - DTO
│   ├── Exceptions/           # Кастомные исключения
│   └── Observability/        # Метрики
├── liquibase/                # Миграции БД
├── prometheus/               # Конфигурация Prometheus
├── MusicWeb.Tests/           # Unit-тесты
├── docker-compose.yml
├── Dockerfile
└── Program.cs
```

## Разработка

### Локальный запуск

```bash
# Запустить зависимости
docker-compose up -d db redis

# Применить миграции
docker-compose up liquibase

# Запустить API
cd MusicWeb
dotnet run
```

### Тесты

```bash
cd MusicWeb.Tests
dotnet test
```

### Сборка Docker образа

```bash
docker build -t musicweb-image -f MusicWeb/Dockerfile .
```

## Метрики

Доступные метрики на `/metrics`:

| Метрика | Описание |
|---------|----------|
| musicweb_songs_requests_total | Количество запросов |
| musicweb_songs_duration_ms | Время выполнения запросов |
| musicweb_songs_cache_hits_total | Попадания в кэш |
| musicweb_songs_cache_misses_total | Промахи кэша |
| musicweb_songs_db_duration_ms | Время запросов к БД |

## Учётные данные по умолчанию

| Сервис | Логин | Пароль |
|--------|-------|--------|
| API (Admin) | admin | admin |
| PostgreSQL | music | music |
| Grafana | admin | admin |
