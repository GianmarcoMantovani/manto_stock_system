using manto_stock_system_API.Entities;
using System.ComponentModel.DataAnnotations;

namespace manto_stock_system_API.DTOs
{
    public class ClientCreationDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public ClientTypeEnum ClientTypeEnum { get; set; }
        public int CityId { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string AttentionHours { get; set; }
    }
}
