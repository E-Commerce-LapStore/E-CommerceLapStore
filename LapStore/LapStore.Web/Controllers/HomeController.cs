using System.Diagnostics;
using LapStore.BLL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using LapStore.BLL.Interfaces;
using System.Linq;

namespace LapStore.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;

        public HomeController(
            ILogger<HomeController> logger,
            ICategoryService categoryService,
            IProductService productService)
        {
            _logger = logger;
            _categoryService = categoryService;
            _productService = productService;
        }
        
        public async Task<IActionResult> Index()
        {
            var homeVM = new HomeVM();
            
            // Get all categories
            var categories = await _categoryService.GetAllCategoriesAsync();
            homeVM.Categories = categories.Select(CategoryVM.FromCategory);

            // Get all products and take the newest 4
            var products = await _productService.GetAllProductsAsync();
            homeVM.NewProducts = products
                .OrderByDescending(p => p.Id) // Assuming Id is auto-incremented
                .Take(4)
                .Select(ProductVM.FromProduct);

            // For now, we'll just take 4 random products as top selling
            // In a real application, this would be based on actual sales data
            homeVM.TopSellingProducts = products
                .OrderBy(x => Guid.NewGuid())
                .Take(4)
                .Select(ProductVM.FromProduct);

            return View(homeVM);
        }
        
        public IActionResult Category()
        {
            return View();
        }
        public IActionResult Product()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
