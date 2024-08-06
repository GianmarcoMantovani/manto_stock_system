using System.ComponentModel.DataAnnotations;

namespace manto_stock_system_API.DTOs
{
    public class LoginDTO

    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

    }
}
