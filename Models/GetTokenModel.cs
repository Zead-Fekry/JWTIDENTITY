using System.ComponentModel.DataAnnotations;

namespace JWT.Models
{
    public class GetTokenModel
    {
        
        public string? Email { get; set; }
        public string? password { get; set; }
    }
}
