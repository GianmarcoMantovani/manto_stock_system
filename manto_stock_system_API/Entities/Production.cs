namespace manto_stock_system_API.Entities
{
    public class Production
    {
        public int Id { get; set; }
        public List<ProductionItem> Items { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}
