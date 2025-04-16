using LapStore.DAL;
using LapStore.DAL.Data.Entities;
using LapStore.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LapStore.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #region GetAll
        public async Task<IActionResult> Index()
        {
            var products = await _unitOfWork.GenericRepository<Product>().GetAllAsync();
            var productVMs = products.Select(ProductVM.FromProduct).ToList();
            return View(productVMs);
        }
        #endregion

        #region Add
        [HttpGet]
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
                await _unitOfWork.GenericRepository<Product>().AddAsync(product);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(productVM);
        }
        #endregion

        #region GetById
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _unitOfWork.GenericRepository<Product>().GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }
            var productVM = ProductVM.FromProduct(product);

            return View(productVM);
        }
        #endregion

        #region Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _unitOfWork.GenericRepository<Product>().GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var productVM = ProductVM.FromProduct(product);
            return View(productVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductVM productVM)
        {
            if (id != productVM.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var product = ProductVM.FromProductVM(productVM);
                _unitOfWork.GenericRepository<Product>().Update(product);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(productVM);
        }
        #endregion

        #region Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _unitOfWork.GenericRepository<Product>().GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var productVM = ProductVM.FromProduct(product);
            return View(productVM);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _unitOfWork.GenericRepository<Product>().GetByIdAsync(id);
            if (product != null)
            {
                _unitOfWork.GenericRepository<Product>().Delete(product);
                await _unitOfWork.CompleteAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        #endregion

    }
}