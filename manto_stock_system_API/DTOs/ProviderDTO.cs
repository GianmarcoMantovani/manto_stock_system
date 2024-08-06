namespace manto_stock_system_API.DTOs
{
    public class ProviderDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public CityDTO City { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Products { get; set; }
        public string AttentionHours { get; set; }
    }
}
