using manto_stock_system_API.Entities;
using System.ComponentModel.DataAnnotations;

namespace manto_stock_system_API.DTOs
{
    public class ProductCreationDTO
    {
        [Required]
        public string Name { get; set; }
        public float Weight { get; set; }
        public string Description { get; set; }
    }
}
