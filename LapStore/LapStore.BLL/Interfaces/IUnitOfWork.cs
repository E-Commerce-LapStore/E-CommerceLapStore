using LapStore.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapStore.BLL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IBaseRepository<T> BaseRepository<T>() where T : class;
        Task<int> CompleteAsync();
    }
}
