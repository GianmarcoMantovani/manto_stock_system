
namespace manto_stock_system_API.DTOs
{
    public class ProductionItemDTO
    {
        public int Id { get; set; }
        public ProductDTO Product { get; set; }
        public int Amount { get; set; }
        public ProductionDTO Production { get; set; }
    }
}
