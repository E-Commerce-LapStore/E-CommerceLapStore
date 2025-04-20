using LapStore.Web.ViewModels;

namespace LapStore.Web.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<CategoryVM> Categories { get; set; }
        public IEnumerable<ProductVM> NewProducts { get; set; }
        public IEnumerable<ProductVM> TopSellingProducts { get; set; }
    }
} 