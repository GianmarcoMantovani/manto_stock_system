using manto_stock_system_API.Entities;

namespace manto_stock_system_API.DTOs
{
    public class ApplicationUserCreationDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName => Email;
        public string Password { get; set; }
        public UserTypeEnum UserType { get; set; }
    }
}
