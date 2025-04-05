using LapStore.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapStore.Core.Interfaces
{
    public interface IProductRepository : IBaseRepository<ProductDto>
    {
        IEnumerable<ProductDto> SpecialMethod();
    }
}
