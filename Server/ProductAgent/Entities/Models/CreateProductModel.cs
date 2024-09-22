using System.ComponentModel.DataAnnotations;

namespace ProductAgent.Entities.Models
{
    public class CreateProductModel
    {
        [Required]
        public string? Name { get; set; }
        public IFormFile? Image { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public int Price { get; set; }
    }
}
