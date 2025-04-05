using LapStore.BLL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LapStore.Web.Configuration
{
    public class DbContextOptionsProvider<TContext> : IDbContextOptionsProvider<TContext> where TContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public DbContextOptionsProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DbContextOptions<TContext> GetDbContextOptions()
        {
            var optionsBuilder = new DbContextOptionsBuilder<TContext>();
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(TContext).Assembly.FullName));

            return optionsBuilder.Options;
        }
    }
}