
namespace manto_stock_system_API.DTOs
{
    public class PurchaseDTO
    {
        public int Id { get; set; }
        public string Products { get; set; }
        public string Description { get; set; }
        public ProviderDTO Provider { get; set; }
        public double Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
