using LapStore.BLL.Interfaces;
using LapStore.BLL.Repositories;
using LapStore.DAL.Entities;
using LapStore.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
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


        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                var product = ProductVM.FromProductVM(productVM);
                await _unitOfWork.BaseRepository<Product>().AddAsync(product);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(productVM);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _unitOfWork.BaseRepository<Product>().GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }
            var productVM = ProductVM.FromProduct(product);

            return View(productVM);
        }
        public async Task<IActionResult> Index()
        {
            var products = await _unitOfWork.BaseRepository<Product>().GetAllAsync();
            var productVMs = products.Select(ProductVM.FromProduct).ToList();
            return View(productVMs);
        }
    }
}