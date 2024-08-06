
using manto_stock_system_API.Entities;

namespace manto_stock_system_API.DTOs
{
    public class SaleItemDTO
    {
        public int Id { get; set; }
        public ProductDTO Product { get; set; }
        public int Amount { get; set; }
        public SaleDTO Sale { get; set; }
    }
}
