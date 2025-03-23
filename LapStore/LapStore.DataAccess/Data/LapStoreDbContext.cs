using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapStore.DataAccess.Data
{
    internal class LapStoreDbContext : DbContext
    {
        public LapStoreDbContext(DbContextOptions<LapStoreDbContext> options) : base(options)
        {

        }
    }
}
