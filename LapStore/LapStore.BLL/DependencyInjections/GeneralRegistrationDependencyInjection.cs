using LapStore.DAL.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using MSConfiguration = Microsoft.Extensions.Configuration;

namespace LapStore.BLL.DependencyInjections
{
    public static class GeneralRegistrationDependencyInjection
    {
        public static IServiceCollection AddGeneralDependencyInjection(this IServiceCollection services, MSConfiguration.IConfiguration configuration)
        {
            // Add services to the container.
            services.AddControllers();

            // Register Swagger/OpenAPI
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });


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
                        sqlServerOptions.ExecutionStrategy(dependencies => 
                            new SqlServerRetryingExecutionStrategy(
                                context: dependencies.CurrentContext.Context,
                                maxRetryCount: 5));
                    });
                
                // Enable diagnostic features
                options.EnableSensitiveDataLogging();
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
            });

            return services;
        }
    }
}
