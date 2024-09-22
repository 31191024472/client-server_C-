using ProductAgent.Entities;
using ProductAgent.Entities.Models;

namespace ProductAgent.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductEntity>> GetProducts(string? name);
        Task<ProductEntity> GetProductByID(int productID);
        Task<(bool, int, string)> CreateProduct(CreateProductModel product);
        Task<(bool, string)> UpdateProduct(UpdateProductModel product);
        Task<bool> DeleteProduct(int productID);
    }
}
