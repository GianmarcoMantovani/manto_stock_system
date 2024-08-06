namespace manto_stock_system_API.Entities
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ClientTypeEnum ClientTypeEnum { get; set; }
        public int CityId { get; set; }
        public City City { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string AttentionHours { get; set; }
    }

    public enum ClientTypeEnum
    {
        Distribuidor,
        Comercio,
        Publico
    }
}
