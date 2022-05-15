using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;

namespace ServerRA_AspnetCore.Model.User
{
    [FirestoreData]
    public class UserInternalModel
    {
        [Required]
        [FirestoreProperty]
        public string? Email { get; set; }

        [Required]
        [FirestoreProperty]
        public string? Name { get; set; }

        [FirestoreProperty]
        public string? DefaultAddress { get; set; }

        [FirestoreProperty]
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
