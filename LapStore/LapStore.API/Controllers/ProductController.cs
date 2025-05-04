using LapStore.BLL.DTOs;
using LapStore.BLL.DTOs.ProductDTO;
using LapStore.BLL.Interfaces;
using LapStore.DAL;
using Microsoft.AspNetCore.Mvc;

namespace LapStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IUnitOfWork _unitOfWork;


        public ProductController(IProductService productService,IUnitOfWork unitOfWork)
        {
            _productService = productService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            var productDTOs = products.Select(ProductReadDTO.FromProduct).ToList();
            return Ok(productDTOs);
        }
        
        [HttpGet("{Id}")]
        public async Task<IActionResult> Details(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            var product = await _productService.GetProductByIdAsync(Id.Value);
            if (product == null)
            {
                return NotFound();
            }
            var productDTO = ProductReadDTO.FromProduct(product);
            return Ok(productDTO);

        }


        [HttpPost]
        public async Task<ActionResult> AddAsync(ProductWriteDTO productDTO)
        {

            if (productDTO == null)
            {
                return BadRequest();
            }

            var product = ProductWriteDTO.FromProductDTO(productDTO);

            await _productService.AddAsync(product);

            return NoContent();
        }

        [HttpPut("{Id}")]
        public ActionResult Edit(int? Id, ProductUpdateDTO productDTO)
        {
            if (Id == null || Id != productDTO.Id || productDTO == null)
                return BadRequest();

            var product = ProductUpdateDTO.FromProductDTO(productDTO);

            _productService.Update(product);

            return NoContent();
        }


        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(int? Id)
        {
            if (Id == null)
                return BadRequest();


            var product = await _productService.GetProductByIdAsync(Id.Value);
            if (product == null)
            {
                return NotFound();
            }

            _productService.Delete(product);
            return NoContent();


        }
    }
}
