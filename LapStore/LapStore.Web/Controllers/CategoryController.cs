using LapStore.BLL.Interfaces;
using LapStore.DAL.Data.Entities;
using LapStore.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LapStore.Web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IFileService _fileService;

        public CategoryController(ICategoryService categoryService, IFileService fileService)
        {
            _categoryService = categoryService;
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
                if (categoryVM.File != null)
                {
                    path = await _fileService.Upload(categoryVM.File, "/Imgs/Categories/");
                    if (path == "Problem")
                    {
                        return BadRequest();
                    }
                }
                categoryVM.ImageUrl = path;

                var category = CategoryVM.FromCategoryVM(categoryVM);
                await _categoryService.AddCategoryAsync(category);
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

            var category = await _categoryService.GetCategoryByIdAsync(id.Value);
            if (category == null)
            {
                return NotFound();
            }
            var categoryVM = CategoryVM.FromCategory(category);
            return View(categoryVM);
        }

        public async Task<IActionResult> Index()
        {
            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"].ToString();
            }
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }

            var categories = await _categoryService.GetAllCategoriesAsync();
            var categoryVMs = categories.Select(CategoryVM.FromCategory).ToList();
            return View(categoryVMs);
        }
        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _categoryService.GetCategoryByIdAsync(id.Value);
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
                try
                {
                    // Handle file upload if a new file is provided
                    if (categoryVM.File != null)
                    {
                        // Upload new image
                        var newImagePath = await _fileService.Upload(categoryVM.File, "/Imgs/Categories/");
                        if (newImagePath == "Problem")
                        {
                            ModelState.AddModelError("", "Error uploading the new image.");
                            return View(categoryVM);
                        }

                        // Delete old image if it exists
                        if (!string.IsNullOrEmpty(categoryVM.ImageUrl))
                        {
                            _fileService.DeletePhysicalFile(categoryVM.ImageUrl);
                        }

                        // Update the image URL
                        categoryVM.ImageUrl = newImagePath;
                    }

                    var category = CategoryVM.FromCategoryVM(categoryVM);
                    await _categoryService.UpdateCategoryAsync(category);
                    TempData["SuccessMessage"] = "Category updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error updating category: " + ex.Message);
                }
            }
            return View(categoryVM);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _categoryService.GetCategoryByIdAsync(id.Value);
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
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);
                if (category != null)
                {
                    await _categoryService.DeleteCategory(category);
                    TempData["SuccessMessage"] = "Category deleted successfully.";
                }
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Cannot delete a category that has products"))
            {
                // Specific error for categories with products
                TempData["ErrorMessage"] = "Cannot delete this category because it contains products. Please delete or move the products to another category first.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // General error handling
                TempData["ErrorMessage"] = "Error deleting category: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> IsCategoryNameExist(string name)
        {
            var result = await _categoryService.IsCategoryNameExistAsync(name);
            if (result)
                return Json(false);
            return Json(true);
        }
    }
}
