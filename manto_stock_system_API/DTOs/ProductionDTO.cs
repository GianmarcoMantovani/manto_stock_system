
namespace manto_stock_system_API.DTOs
{
    public class ProductionDTO
    {
        public int Id { get; set; }
        public List<ProductionItemDTO> Items { get; set; }
        public DateTime Date { get; set; }
        public int TotalProduction { get; set; }
    }
}
