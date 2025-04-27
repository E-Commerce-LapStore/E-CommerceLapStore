using Castle.Core.Configuration;
using LapStore.DAL.Data.Contexts;
using LapStore.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSConfiguration = Microsoft.Extensions.Configuration;

namespace LapStore.BLL.DependencyInjections
{
    public static class GeneralRegistrationDependencyInjection
    {
        public static IServiceCollection AddGeneralDependencyInjection(this IServiceCollection services, MSConfiguration.IConfiguration configuration)
        {
            // Add services to the container.
            services.AddControllersWithViews();

            // Register DbContext with existing database
            services.AddDbContext<LapStoreDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    sqlServerOptions =>
                    {
                        sqlServerOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                        sqlServerOptions.CommandTimeout(30);
                        sqlServerOptions.MigrationsHistoryTable("__EFMigrationsHistory", "dbo");
                    });
                
                // Disable database creation
                options.ConfigureWarnings(warnings => 
                    warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.CoreEventId.ManyServiceProvidersCreatedWarning));
            });

            return services;
        }
    }
}
