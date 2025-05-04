using LapStore.BLL.Interfaces;
using LapStore.BLL.Services;
using LapStore.DAL;
using Microsoft.Extensions.DependencyInjection;

namespace LapStore.BLL.DependencyInjections
{
    public static class ServiceDependencyInjection
    {
        public static IServiceCollection AddServiceDependencyInjection(this IServiceCollection services)
        {
            //Register Services
            // Create one instance for the same request
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IUserService, UserService>();
            return services;
        }
    }
}
