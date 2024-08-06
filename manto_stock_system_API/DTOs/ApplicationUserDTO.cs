using manto_stock_system_API.Entities;

namespace manto_stock_system_API.DTOs
{
    public class ApplicationUserDTO
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public UserTypeEnum UserType { get; set; }
    }
}
