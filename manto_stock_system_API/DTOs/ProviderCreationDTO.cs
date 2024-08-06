using System.ComponentModel.DataAnnotations;

namespace manto_stock_system_API.DTOs
{
    public class ProviderCreationDTO
    {
        [Required]
        public string Name { get; set; }
        public string Company { get; set; }
        public int CityId { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        [Required]
        public string Products { get; set; }
        public string AttentionHours { get; set; }
    }
}
