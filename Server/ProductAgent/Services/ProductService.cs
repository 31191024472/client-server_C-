using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductAgent.Context;
using ProductAgent.Entities;
using ProductAgent.Entities.Models;
using ProductAgent.Interfaces;
using System;
using System.Linq;

namespace ProductAgent.Services
{
    public class ProductService : IProductService
    {
        private readonly ProductDbContext _productDbContext;

        public ProductService(ProductDbContext productDbContext)
        {
            _productDbContext = productDbContext;
        }
        // Tạo mới sản phẩm
        public async Task<(bool, int, string)> CreateProduct(CreateProductModel model)
        {
            if (model.Image == null) return (false, 0, "Vui lòng chọn hình ảnh mô tả sản phẩm");
            var (filePath, message) = await UploadFile(model.Image);
            if (string.IsNullOrEmpty(filePath))
            {
                return (false, 0, message);
            }
            var product = new ProductEntity
            {
                Description = model.Description,
                Name = model.Name,
                Banner = filePath,
                Quantity = model.Quantity,
                Price = model.Price
            };
           await _productDbContext.Products.AddAsync(product);
           await _productDbContext.SaveChangesAsync();
           return (true, product.ID, "Thêm sản phẩm thành công");
        }

        //Xóa sản phẩm
        public async Task<bool> DeleteProduct(int productID)
        {
            var product = await GetProductByID(productID);
            if((product?.ID ?? 0) == 0)
            {
                return false;
            }
            _productDbContext.Products.Remove(product!);
            await _productDbContext.SaveChangesAsync();
            return true;
        }


        // Xem thông tin sản phẩm theo ID
        public async Task<ProductEntity> GetProductByID(int productID)
        {
            return await _productDbContext.Products.FirstOrDefaultAsync(t => t.ID == productID);
        }


        // Xem thông tin sản phẩm theo tên
        public async Task<IEnumerable<ProductEntity>> GetProducts(string? name)
        {
            var products = await _productDbContext.Products.ToListAsync();
            if (!string.IsNullOrEmpty(name))
            {
                products = products.Where(t => t.Name!.Contains(name!, StringComparison.InvariantCultureIgnoreCase)).ToList();
            }
            return products;
        }


        // Update sản phẩm, cập nhật
        public async Task<(bool, string)> UpdateProduct(UpdateProductModel model)
        {
            var productEntity = await GetProductByID(model.ID);
            if((productEntity?.ID ?? 0) == 0)
            {
                return (false, "Không tìm thấy thông tin sản phẩm");
            }
            var product = new ProductEntity
            {
                ID = model.ID,
                Description = model.Description,
                Name = model.Name,
                Banner = productEntity!.Banner,
                Quantity = model.Quantity,
                Price = model.Price
            };
            if(model.Image != null)
            {
                var (filePath, message) = await UploadFile(model.Image!);
                if (string.IsNullOrEmpty(filePath))
                {
                    return (false, message);
                }
                product.Banner = filePath;
            }
            _productDbContext.Products.Update(product);
            await _productDbContext.SaveChangesAsync();
            return (true, "Thành công");

        }


        // Update file hình ảnh
        private async Task<(string, string)> UploadFile(IFormFile file)
        {
            try
            {
                // Kiểm tra loại MIME của tệp
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(file.FileName).ToLower();

                // Kiểm tra phần mở rộng có nằm trong danh sách cho phép không
                if (!allowedExtensions.Contains(extension))
                {
                    return (string.Empty, "Chỉ cho phép tải lên hình ảnh (.jpg, .jpeg, .png, .gif).");
                }
                var fileName = Guid.NewGuid() + extension;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                var fileUrl = $"~/uploads/{fileName}";
                return (fileUrl, "Thành công");
            }
            catch
            {
                return (string.Empty, "Upload hình thất bại");
            }
        }

    }
}
