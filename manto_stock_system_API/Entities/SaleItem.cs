namespace manto_stock_system_API.Entities
{
    public class SaleItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Amount { get; set; }
        public double UnitPrice { get; set; }
        public int SaleId { get; set; }
        public Sale Sale { get; set; }
    }
}
