using Microsoft.EntityFrameworkCore;
using ProductAgent.Entities;

namespace ProductAgent.Context
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
        {
            
        }
        public DbSet<ProductEntity> Products { get; set; }
    }
}
