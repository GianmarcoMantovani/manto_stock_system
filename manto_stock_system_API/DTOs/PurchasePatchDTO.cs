namespace manto_stock_system_API.DTOs
{
    public class PurchasePatchDTO
    {
        public string Products { get; set; }
        public string Description { get; set; }
        public int ProviderId { get; set; }
        public double Amount { get; set; }
    }
}
