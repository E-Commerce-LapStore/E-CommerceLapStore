using LapStore.BLL.Interfaces;
using LapStore.BLL.Services;
using LapStore.DAL;
using LapStore.DAL.Data.Entities;
using LapStore.DAL.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapStore.BLL.DependencyInjections
{
    public static class RepositoryDependencyInjection
    {
        public static IServiceCollection AddRepositoryDependencyInjection(this IServiceCollection services)
        {
            // Register repositories

            services.AddScoped<IGenericRepository<Address>, GenericRepository<Address>>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            return services;
        }
    }
}
