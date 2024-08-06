namespace manto_stock_system_API.Entities
{
    public class Sale
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<SaleItem> Items { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public double TotalPrice { get; set; }
    }
}
