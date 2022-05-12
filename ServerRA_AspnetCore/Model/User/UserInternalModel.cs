using System.ComponentModel.DataAnnotations;

namespace ServerRA_AspnetCore.Model.User
{
    public class UserInternalModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }

        public string DefaultAddress { get; set; }

        public string Role { get; set; }
    }
}
