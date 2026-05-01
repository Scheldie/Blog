# 📝 Blog Platform - ASP.NET Core MVC

<div align="center">
  <img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/dotnetcore/dotnetcore-original.svg" width="60" height="60"/>
  <img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/postgresql/postgresql-original.svg" width="60" height="60"/>
  <img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/docker/docker-original.svg" width="60" height="60"/>
  <img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/csharp/csharp-original.svg" width="60" height="60"/>
</div>

Современная платформа для ведения блога с полным набором социальных функций, построенная на ASP.NET Core MVC с использованием Entity Framework Core и PostgreSQL.

## 🌟 Основные возможности

### 👤 Управление профилем
- Регистрация и аутентификация пользователей
- Редактирование профиля (имя, аватар, информация о себе)
- Просмотр профилей других пользователей

### 📝 Публикации
- Создание, редактирование и удаление постов
- Загрузка до 4 изображений к посту
- Карусель для просмотра постов пользователя
- Лента публикаций всех пользователей

### 💬 Комментарии
- Современная система комментариев с древовидной структурой
- Ответы на комментарии других пользователей
- Редактирование и удаление своих комментариев
- Лайки к комментариям

### 🔍 Поиск
- Поиск пользователей по имени
- Просмотр всех результатов поиска

### 🛠 Технологии
<div align="left">
  <img src="https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white" alt=".NET"/>
  <img src="https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white" alt="PostgreSQL"/>
  <img src="https://img.shields.io/badge/Docker-2CA5E0?style=for-the-badge&logo=docker&logoColor=white" alt="Docker"/>
  <img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white" alt="C#"/>
</div>

## 🚀 Быстрый старт

### Требования
- <img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/dotnetcore/dotnetcore-original.svg" width="20" height="20"/> [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
- <img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/docker/docker-original.svg" width="20" height="20"/> [Docker](https://www.docker.com/get-started)
- <img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/docker/docker-original.svg" width="20" height="20"/> [Docker Compose](https://docs.docker.com/compose/install/)

### Установка и запуск

**Клонируйте репозиторий:**

```bash

git clone https://github.com/Scheldie/Blog.git
cd Blog
```
**Запустите контейнер с помощью Docker Compose:**

```bash

docker-compose up --build
```
После этого запустите приложение

**Приложение будет доступно по адресу:**

```text

https://localhost:7141
```
### Конфигурация

**Настройки приложения можно изменить в файле appsettings.json:**
```json

{
  "ConnectionStrings": {
    "DefaultConnection": "Host=db;Port=5432;Database=blogdb;Username=postgres;Password=yourpassword"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```
### 📂 Структура проекта
```text

Blog/
├── Controllers/        # Контроллеры MVC
├── Data/               # Контекст БД и репозитории
├── Entities/           # Сущности
├── Models/             # Модели
├── Services/           # Сервисы приложения
├── Migrations/         # Миграции
├── Views/              # Представления Razor
├── wwwroot/            # Статические файлы
└── Program.cs          # Точка входа
```
### 🌍 Особенности архитектуры

    Чистая архитектура с разделением на слои

    Асинхронные методы на I/O

    Валидация на стороне клиента и сервера

### 🤝 Участие в разработке

### Мы приветствуем вклад в развитие проекта!

    Форкните репозиторий

    Создайте ветку для вашей фичи (git checkout -b feature/AmazingFeature)

    Сделайте коммит изменений (git commit -m 'Add some AmazingFeature')

    Запушьте в ветку (git push origin feature/AmazingFeature)

    Откройте Pull Request

### 📜 Лицензия

Этот проект распространяется под лицензией MIT. Подробнее см. в файле LICENSE.

Автор: [Scheldie](https://github.com/Scheldie)
Версия: 1.0.0
Дата: 2025

**Краткая иллюстрация**
https://github.com/user-attachments/assets/93590e64-5286-4167-baa9-aa386f0882f3
