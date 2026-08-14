using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyTodo.Application.Repository.Interface;
using MyTodo.Infrastructure.Persistence.DbContext;
using MyTodo.Infrastructure.Persistence.Repositories;

namespace MyTodo.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<MyTodoDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<ILifeAreaRepository, LifeAreaRepository>();
            services.AddScoped<IProblemRepository, ProblemRepository>();
            services.AddScoped<IProblemStatusOrderRepository, ProblemStatusOrderRepository>();
            services.AddScoped<ISolutionRepository, SolutionRepository>();
            services.AddScoped<IObjectiveRepository, ObjectiveRepository>();
            services.AddScoped<ITodoTaskRepository, TodoTaskRepository>();
            services.AddScoped<IExperimentRepository, ExperimentRepository>();
            services.AddScoped<ISprintRepository, SprintRepository>();
            services.AddScoped<ITodoRepository, TodoRepository>();

            return services;
        }
    }
}
