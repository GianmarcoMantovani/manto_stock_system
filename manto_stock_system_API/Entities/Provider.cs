namespace manto_stock_system_API.Entities
{
    public class Provider
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public int CityId { get; set; }
        public City City { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Products { get; set; }
        public string AttentionHours { get; set; }
    }
}
