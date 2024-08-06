namespace manto_stock_system_API.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Stock {  get; set; } = 0;
        public float Weight { get; set; }
        public string Description { get; set; }
    }
}
