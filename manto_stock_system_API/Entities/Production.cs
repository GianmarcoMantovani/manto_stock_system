namespace manto_stock_system_API.Entities
{
    public class Production
    {
        public int Id { get; set; }
        public List<ProductionItem> Items { get; set; }
        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}
