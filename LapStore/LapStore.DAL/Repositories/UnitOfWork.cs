using AutoMapper;
using LapStore.Core.DTOs;
using LapStore.Core.Interfaces;
using LapStore.DAL.Contexts;
using LapStore.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapStore.DAL.Repositories
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


        public IBaseRepository<UserDto> users { get; private set; }

        IBaseRepository<UserDto> IUnitOfWork.users => throw new NotImplementedException();

        public IProductRepository products => throw new NotImplementedException();

        

        public int Complete()
        {   
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
