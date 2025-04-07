using LapStore.BLL.Interfaces;
using LapStore.DAL.Entities;
using LapStore.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LapStore.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _unitOfWork.BaseRepository<Product>().GetAllAsync();
            var productVMs = new List<ProductVM>();
            foreach (var product in products)
            {
                productVMs.Add(new ProductVM(product));
            }
            return View(productVMs);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    Name = productVM.Name,
                    Description = productVM.Description,
                    Price = productVM.Price
                };
                _unitOfWork.BaseRepository<Product>().AddAsync(product);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(productVM);
        }

        public IActionResult GetView()
        {
            return View("MyView");
        }
    }
}