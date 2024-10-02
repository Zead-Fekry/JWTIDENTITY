using System.ComponentModel.DataAnnotations;

namespace JWT.Services
{
    public class RegisterModel
    {
        [Required,StringLength(100)]
        public String FirstName { get; set; }

        [Required, StringLength(100)]
        public String LastName { get; set; }

        [Required, StringLength(50)]
        public String UserName { get; set; }

        [Required, StringLength(128)]
        public String Email { get; set; }

        [Required, StringLength(256)]
        public String Password { get; set; }
    }
}
