using LapStore.BLL.DTOs;

namespace LapStore.BLL.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<CategoryVM> Categories { get; set; }
        public IEnumerable<ProductDTO> NewProducts { get; set; }
        public IEnumerable<ProductDTO> TopSellingProducts { get; set; }
    }
} 