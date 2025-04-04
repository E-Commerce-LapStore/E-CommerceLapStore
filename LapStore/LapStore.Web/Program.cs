using LapStore.Core.Interfaces;
using LapStore.DAL.Contexts;
using LapStore.Web.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;

namespace LapStore.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Register the provider (using generic type)
            builder.Services.AddScoped(typeof(IDbContextOptionsProvider<>), typeof(DbContextOptionsProvider<>));

            // Register the DbContext using the provider
            builder.Services.AddDbContext<LapStoreDbContext>(options =>
            {
                var serviceProvider = builder.Services.BuildServiceProvider();
                var provider = serviceProvider.GetRequiredService<IDbContextOptionsProvider<LapStoreDbContext>>();
                var contextOptions = provider.GetDbContextOptions();
                options.UseSqlServer(contextOptions.FindExtension<SqlServerOptionsExtension>().ConnectionString,
                    b => b.MigrationsAssembly(typeof(LapStoreDbContext).Assembly.FullName));
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.UseStaticFiles();

            app.Run();
        }
    }
}