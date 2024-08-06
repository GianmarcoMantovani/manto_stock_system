namespace manto_stock_system_API.Entities
{
    public class ProductionItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Amount { get; set; }
        public int ProductionId { get; set; }
        public Production Production { get; set; }
    }
}
