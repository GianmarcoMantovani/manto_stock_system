using manto_stock_system_API.Entities;

namespace manto_stock_system_API.DTOs
{
    public class ApplicationUserPatchDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserTypeEnum UserType { get; set; }
    }
}
