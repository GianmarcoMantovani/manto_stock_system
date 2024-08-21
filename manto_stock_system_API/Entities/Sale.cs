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
        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Today);
        public double TotalPrice { get; set; }
        public bool Sold { get; set; }    
    }
}
