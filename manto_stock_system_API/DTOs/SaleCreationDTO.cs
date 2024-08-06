
namespace manto_stock_system_API.DTOs
{
    public class SaleCreationDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<SaleItemCreationDTO> Items { get; set; }
        public int ClientId { get; set; }
        public double TotalPrice { get; set; }
    }
}
