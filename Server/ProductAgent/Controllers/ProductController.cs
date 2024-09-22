using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductAgent.Entities;
using ProductAgent.Entities.Models;
using ProductAgent.Interfaces;


namespace ProductAgent.Controllers
{
    [Route("api/products/")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpGet]
        public async Task<IActionResult> GetProducts(string? name)
        {
            //var ProductService = new ProductService();
            var products = await _productService.GetProducts(name);
            return Ok(products);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductByID(int id)
        {
            var product = await _productService.GetProductByID(id);
            return Ok(product);
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateProduct(CreateProductModel product)
        {
            var (isSuccess, productID, message) = await _productService.CreateProduct(product);
            return Ok(new { IsSuccess = isSuccess, ProductID = productID, Message = message });
        }
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateProduct(UpdateProductModel product, int id)
        {
            product.ID = id;
            var (isSuccess, message) = await _productService.UpdateProduct(product);
            return Ok(new { IsSuccess = isSuccess, Message = message });
        }
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var isSuccess = await _productService.DeleteProduct(id);
            return Ok(new { IsSuccess = isSuccess, Message = "Thành công" });
        }
    }
}
