using manto_stock_system_API.Entities;

namespace manto_stock_system_API.DTOs
{
    public class ClientDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ClientTypeEnum ClientTypeEnum { get; set; }
        public CityDTO City { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string AttentionHours { get; set; }
    }
}
