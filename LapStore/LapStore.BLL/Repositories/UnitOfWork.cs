using AutoMapper;
using LapStore.BLL.Interfaces;
using LapStore.DAL.Contexts;
using LapStore.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapStore.BLL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly LapStoreDbContext _context;

        public UnitOfWork(LapStoreDbContext context)// : base(context)
        {
            _context = context;
        }

        private readonly IMapper _mapper; //  AutoMapper

        public UnitOfWork(LapStoreDbContext context, IMapper mapper)// : base(context)
        {
            _context = context;
            _mapper = mapper;
        }




        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
