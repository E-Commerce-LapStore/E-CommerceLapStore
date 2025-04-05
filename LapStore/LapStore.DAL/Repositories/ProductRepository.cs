using AutoMapper;
using LapStore.Core.DTOs;
using LapStore.Core.Interfaces;
using LapStore.DAL.Contexts;
using LapStore.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LapStore.DAL.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        private readonly LapStoreDbContext _context;


        private readonly IMapper _mapper; // AutoMapper

        public ProductRepository(LapStoreDbContext context, IMapper mapper) : base(context)
        {
            _context = context;
            _mapper = mapper;
        }

        public IEnumerable<ProductDto> SpecialMethod()
        {
            var _products = _context.products.Where(p => p.Price > 100).ToList();
            return (IEnumerable<ProductDto>)_mapper.Map<IEnumerable<ProductDto>>(_products);
        }

        ProductDto IBaseRepository<ProductDto>.GetById(int id)
        {
            throw new NotImplementedException();
        }

        IEnumerable<ProductDto> IBaseRepository<ProductDto>.GetAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ProductDto> Find(Expression<Func<ProductDto, bool>> criteria)
        {
            throw new NotImplementedException();
        }

        public void Add(ProductDto entity)
        {
            throw new NotImplementedException();
        }

        public void Update(ProductDto entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(ProductDto entity)
        {
            throw new NotImplementedException();
        }


    }
}
