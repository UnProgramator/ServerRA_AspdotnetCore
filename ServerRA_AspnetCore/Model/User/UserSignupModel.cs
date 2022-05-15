using System.ComponentModel.DataAnnotations;

namespace ServerRA_AspnetCore.Model.User
{
    public class UserSignupModel : UserAuthenticationModel
    {
        //[Required]
        //[EmailAddress]
        //public string Email { get; set; } //inherit from auth

        //[Required]
        //public string Password { get; set; } //inherit from auth

        [Required]
        public string Name { get; set; }

        public string DefaultAddress { get; set; }

        public UserInternalModel getUserPublicInfo()
        {
            return new UserInternalModel
            {
                Name = Name,
                DefaultAddress = DefaultAddress,
                Role = "Email"
            };
        }

    }
}
