using LapStore.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapStore.DAL.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        public T GetByID(int id)
        {
            throw new NotImplementedException();
        }
    }
}
