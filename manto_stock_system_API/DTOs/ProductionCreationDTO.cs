using System.ComponentModel.DataAnnotations;

namespace manto_stock_system_API.DTOs
{
    public class ProductionCreationDTO
    {
        [Required]
        public List<ProductionItemCreationDTO> Items { get; set; }
    }
}
