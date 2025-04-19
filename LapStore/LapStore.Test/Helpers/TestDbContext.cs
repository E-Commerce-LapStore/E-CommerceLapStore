using LapStore.DAL.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LapStore.Test.Helpers
{
    public class TestDbContext : LapStoreDbContext
    {
        public TestDbContext(DbContextOptions<LapStoreDbContext> options) : base(options)
        {
        }

        public DbSet<TestEntity> TestEntities { get; set; }
    }
}