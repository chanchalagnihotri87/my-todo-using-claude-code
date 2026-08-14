using Microsoft.Extensions.DependencyInjection;
using MyTodo.Application.Services;
using MyTodo.Application.Services.Interfaces;

namespace MyTodo.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ILifeAreaService, LifeAreaService>();
            services.AddScoped<IProblemService, ProblemService>();
            services.AddScoped<IProblemStatusOrderService, ProblemStatusOrderService>();
            services.AddScoped<ISolutionService, SolutionService>();
            services.AddScoped<IObjectiveService, ObjectiveService>();
            services.AddScoped<ITodoTaskService, TodoTaskService>();
            services.AddScoped<IExperimentService, ExperimentService>();
            services.AddScoped<ISprintService, SprintService>();
            services.AddScoped<ITodoService, TodoService>();

            return services;
        }
    }
}
