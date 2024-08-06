namespace manto_stock_system_API.Entities
{
    public class Purchase
    {
        public int Id { get; set; }
        public string Products { get; set; }
        public string Description { get; set; }
        public int ProviderId { get; set; }
        public Provider Provider { get; set; }
        public double Amount { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}
