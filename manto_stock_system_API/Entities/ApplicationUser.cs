using Microsoft.AspNetCore.Identity;

namespace manto_stock_system_API.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public UserTypeEnum UserType { get; set; }
    }

    public enum UserTypeEnum
    {
        Admin, // 0
        User  // 1
    }
}
