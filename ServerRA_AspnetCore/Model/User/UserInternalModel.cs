using System.ComponentModel.DataAnnotations;

namespace ServerRA_AspnetCore.Model.User
{
    public class UserInternalModel
    {
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Name { get; set; }

        public string? DefaultAddress { get; set; }

        public string? Role { get; set; }

        public void complete (UserInternalModel? other)
        {
            if (other == null) return;

            if (Email == null)
            {
                Email = other.Email;
            }

            if (Name == null)
            {
                Name = other.Name;
            }

            if (DefaultAddress == null)
            {
                DefaultAddress = other.DefaultAddress;
            }

            if (Role == null)
            {
                Role = other.Role;
            }
        }
    }
}
