using LapStore.DAL.Data.Entities;
using LapStore.DAL.Repositories;
using LapStore.BLL.Interfaces;
using LapStore.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace LapStore.BLL.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IFileService _fileService;
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IProductRepository productRepository, IFileService fileService, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _fileService = fileService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Product> GetProductByNameAsync(string name)
        {
            return await _productRepository.GetProductByNameAsync(name);
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _productRepository.GetProductsByCategoryAsync(categoryId);
        }

        public async Task AddProductAsync(Product product, IFormFile mainImageFile, List<IFormFile>? additionalImageFiles)
        {
            try
            {
                // First add the product to get its ID
                await _productRepository.AddAsync(product);
                await _unitOfWork.CompleteAsync();

                product.productImages = new List<ProductImage>();

                // Handle main image
                if (mainImageFile != null)
                {
                    var mainImageUrl = await _fileService.Upload(mainImageFile, "/Imgs/Products/");
                    if (mainImageUrl != "Problem")
                    {
                        product.productImages.Add(new ProductImage
                        {
                            ProductId = product.Id,
                            URL = mainImageUrl,
                            IsMain = true
                        });
                    }
                }

                // Then handle additional image uploads if any
                if (additionalImageFiles != null && additionalImageFiles.Any())
                {
                    foreach (var imageFile in additionalImageFiles)
                    {
                        var imageUrl = await _fileService.Upload(imageFile, "/Imgs/Products/");
                        if (imageUrl != "Problem")
                        {
                            product.productImages.Add(new ProductImage
                            {
                                ProductId = product.Id,
                                URL = imageUrl,
                                IsMain = false
                            });
                        }
                    }
                }

                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding product: {ex.Message}", ex);
            }
        }

        public async Task UpdateProductAsync(Product product, IFormFile? mainImageFile, List<IFormFile>? additionalImageFiles, List<string>? imagesToDelete)
        {
            try
            {
                var existingProduct = await _productRepository.GetByIdAsync(product.Id);
                if (existingProduct == null)
                    throw new Exception("Product not found");

                // Update basic product information
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.Weight = product.Weight;
                existingProduct.CategoryId = product.CategoryId;

                // Handle image deletions if any
                if (imagesToDelete != null && imagesToDelete.Any())
                {
                    foreach (var imageUrl in imagesToDelete)
                    {
                        var imageToDelete = existingProduct.productImages?.FirstOrDefault(pi => pi.URL == imageUrl);
                        if (imageToDelete != null)
                        {
                            // First delete the physical file
                            bool fileDeleted = _fileService.DeletePhysicalFile(imageUrl);
                            
                            // Then remove from database regardless of file deletion result
                            existingProduct.productImages.Remove(imageToDelete);
                            _productRepository.RemoveProductImage(imageToDelete);
                        }
                    }
                }

                // Handle main image update if provided
                if (mainImageFile != null)
                {
                    // Remove existing main image if any
                    var existingMainImage = existingProduct.productImages?.FirstOrDefault(pi => pi.IsMain);
                    if (existingMainImage != null)
                    {
                        bool fileDeleted = _fileService.DeletePhysicalFile(existingMainImage.URL);
                        existingProduct.productImages.Remove(existingMainImage);
                        _productRepository.RemoveProductImage(existingMainImage);
                    }

                    // Add new main image
                    var mainImageUrl = await _fileService.Upload(mainImageFile, "/Imgs/Products/");
                    if (mainImageUrl != "Problem")
                    {
                        if (existingProduct.productImages == null)
                            existingProduct.productImages = new List<ProductImage>();

                        existingProduct.productImages.Add(new ProductImage
                        {
                            ProductId = existingProduct.Id,
                            URL = mainImageUrl,
                            IsMain = true
                        });
                    }
                }

                // Handle additional image uploads if any
                if (additionalImageFiles != null && additionalImageFiles.Any())
                {
                    if (existingProduct.productImages == null)
                        existingProduct.productImages = new List<ProductImage>();

                    foreach (var imageFile in additionalImageFiles)
                    {
                        var imageUrl = await _fileService.Upload(imageFile, "/Imgs/Products/");
                        if (imageUrl != "Problem")
                        {
                            existingProduct.productImages.Add(new ProductImage
                            {
                                ProductId = existingProduct.Id,
                                URL = imageUrl,
                                IsMain = false
                            });
                        }
                    }
                }

                _productRepository.Update(existingProduct);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating product: {ex.Message}", ex);
            }
        }

        public async Task DeleteProduct(Product product)
        {
            try
            {
                // Check if the product has any related records
                if ((product.cartItems ?? Enumerable.Empty<CartItem>()).Any() ||
                    (product.orderItems ?? Enumerable.Empty<OrderItem>()).Any() ||
                    (product.productReviews ?? Enumerable.Empty<Review>()).Any())
                {
                    throw new InvalidOperationException("Cannot delete a product that has related records (cart items, order items, or reviews).");
                }

                // Delete associated product images
                if (product.productImages != null)
                {
                    foreach (var image in product.productImages)
                    {
                        if (!string.IsNullOrEmpty(image.URL))
                        {
                            bool fileDeleted = _fileService.DeletePhysicalFile(image.URL);
                            if (!fileDeleted)
                            {
                                System.Diagnostics.Debug.WriteLine($"Warning: Could not delete file {image.URL}");
                            }
                        }
                        // Remove the image entity using the repository
                        _productRepository.RemoveProductImage(image);
                    }
                }

                // Then delete the product
                _productRepository.Delete(product);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting product: {ex.Message}", ex);
            }
        }

        public async Task<bool> IsProductNameExistAsync(string productName)
        {
            var product = await _productRepository.GetProductByNameAsync(productName);
            return product != null;
        }
    }
} 