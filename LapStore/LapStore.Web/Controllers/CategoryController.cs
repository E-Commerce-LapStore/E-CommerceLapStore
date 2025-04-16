using LapStore.BLL.Interfaces;
using LapStore.DAL;
using LapStore.DAL.Data.Entities;
using LapStore.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LapStore.Web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;

        public CategoryController(IUnitOfWork unitOfWork, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CategoryVM categoryVM)
        {
            if (ModelState.IsValid)
            {
                var path = "";
                if (categoryVM.File.Length > 0)
                {
                    path = await _fileService.Upload(categoryVM.File, "/Imgs/Categories/");
                    if (path == "Problem")
                    {
                        return BadRequest();
                    }
                }
                categoryVM.ImageUrl = path;

                var category = CategoryVM.FromCategoryVM(categoryVM);
                await _unitOfWork.GenericRepository<Category>().AddAsync(category);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(categoryVM);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _unitOfWork.GenericRepository<Category>().GetByIdAsync(id);

            if (category == null)
            {
                return NotFound();
            }
            var categoryVM = CategoryVM.FromCategory(category);

            return View(categoryVM);
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _unitOfWork.GenericRepository<Category>().GetAllAsync();
            var categoryVMs = categories.Select(CategoryVM.FromCategory).ToList();
            return View(categoryVMs);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _unitOfWork.GenericRepository<Category>().GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var categoryVM = CategoryVM.FromCategory(category);
            return View(categoryVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryVM categoryVM)
        {
            if (id != categoryVM.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var category = CategoryVM.FromCategoryVM(categoryVM);
                _unitOfWork.GenericRepository<Category>().Update(category);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(categoryVM);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _unitOfWork.GenericRepository<Category>().GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var categoryVM = CategoryVM.FromCategory(category);
            return View(categoryVM);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _unitOfWork.GenericRepository<Category>().GetByIdAsync(id);
            if (category != null)
            {
                _unitOfWork.GenericRepository<Category>().Delete(category);
                await _unitOfWork.CompleteAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
