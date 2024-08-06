using System.ComponentModel.DataAnnotations;

namespace manto_stock_system_API.DTOs
{
    public class ProductPatchDTO
    {
        public string Name { get; set; }
        public int Stock { get; set; }
        public float Weight { get; set; }
        public string Description { get; set; }
    }
}
