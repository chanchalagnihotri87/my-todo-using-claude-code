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

            return services;
        }
    }
}
