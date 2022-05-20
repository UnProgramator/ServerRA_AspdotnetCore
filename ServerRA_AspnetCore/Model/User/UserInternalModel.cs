using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;

namespace ServerRA_AspnetCore.Model.User
{
    [FirestoreData]
    public class UserInternalModel
    {

        [Required]
        [FirestoreProperty]
        public string? name { get; set; }

        [FirestoreProperty]
        public string? defaultAddress { get; set; }

        [FirestoreProperty]
        public string? role { get; set; }

        public Dictionary<string, object> getAsDict(bool ignoreNull = true, bool returnRole = false)
        {
            Dictionary<string, object> retVal = new Dictionary<string, object>();

            if(!ignoreNull || name != null) retVal.Add("Name", name);
            if(!ignoreNull || defaultAddress != null) retVal.Add("DefaultAddress", defaultAddress);
            if(returnRole && (!ignoreNull || role != null)) retVal.Add("Role", role);

            return retVal;
        }


    }
}
