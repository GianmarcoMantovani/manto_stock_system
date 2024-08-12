
namespace manto_stock_system_API.DTOs
{
    public class SaleItemCreationDTO
    {
        public int ProductId { get; set; }
        public int Amount { get; set; }
        public int SaleId { get; set; }
        public double UnitPrice { get; set; }
    }
}
