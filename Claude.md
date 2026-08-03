# Project: MyTodo Personal Assistant

## What this project does
Asp.Net MVC project with Clean Architecture for a personal assistant application that helps users manage their tasks and to-do lists efficiently.
User can manage their whole life, such as Life Areas -> Problems -Solutions -> Tasks -> Move to Sprint -> Done. 
The application allows users to create, read, update, and delete tasks, set reminders, and categorize tasks based on priority or project.


## Tech Stack
- ASP.NET Core MVC, .Net 10
- Entity Framework Core + SQL Server (Server Name: localhost\\MSSQLSERVER01, Database Name: MyTodoDB_GenerateClaudeMdByOwn)


## Project Structure
- `MyTodo` : The main ASP.NET Core MVC project that contains the presentation layer, controllers, views, and models.
- `MyTodo.Application` : Contains the application layer, including services, interfaces, and DTOs.
- `MyTodo.Domain` : Contains the domain layer, including entities, value objects, and domain services.
- `MyTodo.Infrastructure` : Contains the infrastructure layer, including data access, repositories, and external services.


## Coding Conventions
- Follow PascalCase for naming public members and methods. Use _camelCase for private fields
- Use meaningful names for variables, methods, and classes to improve code readability.
- Use async/await for asynchronous operations to improve performance and responsiveness.


## Commands
- `dotnet build` : Build the application
- `dotnet run --project MyTodo` : Run the application
- `dotnet ef migrations add <MigrationName>` : Add a new migration
- `dotnet ef database update` : Apply migrations to the database


## Don'ts
- Don't modify `Migrations` folder manually. Use EF Core commands to manage migrations.
- Don't add nuget packages without asking.