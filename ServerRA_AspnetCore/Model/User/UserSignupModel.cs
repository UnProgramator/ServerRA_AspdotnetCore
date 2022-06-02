using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;

namespace ServerRA_AspnetCore.Model.User
{
    [FirestoreData]
    public class UserSignupModel : UserAuthenticationModel
    {
        [Required]
        [FirestoreProperty]
        public string name { get; set; } = "";

        [FirestoreProperty]
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
