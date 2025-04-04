using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace LapStore.Core.Interfaces
{
    public interface IDbContextOptionsProvider<TContext> where TContext : DbContext
    {
        DbContextOptions<TContext> GetDbContextOptions();
    }
}
