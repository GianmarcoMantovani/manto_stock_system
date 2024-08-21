
namespace manto_stock_system_API.DTOs
{
    public class SaleDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<SaleItemDTO> Items { get; set; }
        public ClientDTO Client { get; set; }
        public DateOnly Date { get; set; }
        public double TotalPrice { get; set; }
        public bool Sold { get; set; }
    }
}
