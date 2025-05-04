using LapStore.BLL.DTOs;
using LapStore.BLL.Interfaces;
using LapStore.DAL.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nest;

namespace LapStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(
            IProductService productService,
            ICategoryService categoryService,
            ILogger<ProductController> logger)
        {
            _productService = productService;
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
                var products = await _productService.GetAllProductsAsync();
                var productDTOs = products.Select(ProductDTO.FromProduct).ToList();
                return Ok(productDTOs);
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productService.GetProductByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }
            var productDTO = ProductDTO.FromProduct(product);
            return Ok(productDTO);
            
        }


        [HttpPost("{Id}")]
        public async Task<ActionResult> AddAsync(ProductDTO productDTO)
        {
            var product = ProductDTO.FromProductDTO(productDTO);

            await _productService.AddAsync(product);

            return NoContent();
        }

        [HttpPut("{Id}")]
        public ActionResult Edit(int Id, ProductDTO productDTO)
        {
            if (Id != productDTO.Id)
                return BadRequest();

            var product = ProductDTO.FromProductDTO(productDTO);

            _productService.Update(product);

            return NoContent();
        }


        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var product = await _productService.GetProductByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            _productService.Delete(product);

            return NoContent();


        }
    }
}
