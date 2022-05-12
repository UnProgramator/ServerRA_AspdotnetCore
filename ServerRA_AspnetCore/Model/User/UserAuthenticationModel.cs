using System.ComponentModel.DataAnnotations;

namespace ServerRA_AspnetCore.Model.User
{
    public class UserAuthenticationModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
