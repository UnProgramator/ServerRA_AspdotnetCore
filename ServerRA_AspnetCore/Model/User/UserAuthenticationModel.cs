using System.ComponentModel.DataAnnotations;

namespace ServerRA_AspnetCore.Model.User
{
    public class UserAuthenticationModel
    {
        [Required]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        public string password { get; set; }
    }
}
