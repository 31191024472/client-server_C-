using System.ComponentModel.DataAnnotations;

namespace ProductAgent.Entities.Models
{
    public class UpdateProductModel
    {
        [Required]
        public int ID { get; set; }
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
