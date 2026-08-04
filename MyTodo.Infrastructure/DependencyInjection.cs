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

            return services;
        }
    }
}
