using System.ComponentModel.DataAnnotations;

namespace ServerRA_AspnetCore.Model.User
{
    public class UserSignupModel : UserAuthenticationModel
    {
        //[Required]
        //[EmailAddress]
        //public string email { get; set; } //inherit from auth

        //[Required]
        //public string password { get; set; } //inherit from auth

        [Required]
        public string name { get; set; } = "";

        public string defaultAddress { get; set; } = "";

        public UserInternalModel getUserPublicInfo()
        {
            return new UserInternalModel
            {
                name = name,
                defaultAddress = defaultAddress,
                role = "Email"
            };
        }

    }
}
