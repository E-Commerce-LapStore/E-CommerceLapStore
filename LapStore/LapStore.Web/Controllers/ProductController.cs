using LapStore.BLL.Interfaces;
using LapStore.DAL.Data.Entities;
using LapStore.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LapStore.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        //pla pla pla
        public ProductController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
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

            var products = await _productService.GetAllProductsAsync();
            var productVMs = products.Select(ProductVM.FromProduct).ToList();
            return View(productVMs);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "Name");
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading categories: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(ProductVM productVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (productVM.MainImageFile == null)
                    {
                        ModelState.AddModelError("MainImageFile", "Main image is required when adding a new product");
                        var categories = await _categoryService.GetAllCategoriesAsync();
                        ViewBag.Categories = new SelectList(categories, "Id", "Name", productVM.CategoryId);
                        return View(productVM);
                    }

                    var product = ProductVM.FromProductVM(productVM);
                    await _productService.AddProductAsync(product, productVM.MainImageFile, productVM.AdditionalImageFiles);
                    TempData["SuccessMessage"] = "Product added successfully.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error adding product: " + ex.Message);
            }

            // If we got this far, something failed; redisplay form
            var categoriesForError = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Categories = new SelectList(categoriesForError, "Id", "Name", productVM.CategoryId);
            return View(productVM);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var product = await _productService.GetProductByIdAsync(id.Value);
                if (product == null)
                {
                    return NotFound();
                }
                var productVM = ProductVM.FromProduct(product);
                return View(productVM);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading product details: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var product = await _productService.GetProductByIdAsync(id.Value);
                if (product == null)
                {
                    return NotFound();
                }

                var categories = await _categoryService.GetAllCategoriesAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
                
                var productVM = ProductVM.FromProduct(product);
                return View(productVM);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading product for editing: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductVM productVM, List<string>? imagesToDelete)
        {
            if (id != productVM.Id)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var product = ProductVM.FromProductVM(productVM);
                    await _productService.UpdateProductAsync(product, productVM.MainImageFile, productVM.AdditionalImageFiles, imagesToDelete);
                    TempData["SuccessMessage"] = "Product updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error updating product: " + ex.Message);
            }

            // If we got this far, something failed; redisplay form
            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", productVM.CategoryId);
            return View(productVM);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var product = await _productService.GetProductByIdAsync(id.Value);
                if (product == null)
                {
                    return NotFound();
                }

                var productVM = ProductVM.FromProduct(product);
                return View(productVM);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading product for deletion: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product != null)
                {
                    await _productService.DeleteProduct(product);
                    TempData["SuccessMessage"] = "Product deleted successfully.";
                }
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Cannot delete a product that has related records"))
            {
                TempData["ErrorMessage"] = "Cannot delete this product because it has related records (cart items, order items, or reviews).";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error deleting product: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> IsProductNameExist(string name, int? id)
        {
            var product = await _productService.GetProductByNameAsync(name);
            if (product == null)
                return Json(true);
            
            // If we're editing an existing product, the name is valid if it belongs to the same product
            if (id.HasValue && product.Id == id.Value)
                return Json(true);

            return Json(false);
        }
    }
}