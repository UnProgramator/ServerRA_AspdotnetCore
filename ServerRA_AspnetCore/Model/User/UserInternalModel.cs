using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;

namespace ServerRA_AspnetCore.Model.User
{
    [FirestoreData]
    public class UserInternalModel
    {

        [Required]
        [FirestoreProperty]
        public string? Name { get; set; }

        [FirestoreProperty]
        public string? DefaultAddress { get; set; }

        [FirestoreProperty]
        public string? Role { get; set; }

        public Dictionary<string, object> getAsDict(bool ignoreNull = true, bool returnRole = false)
        {
            Dictionary<string, object> retVal = new Dictionary<string, object>();

            if(!ignoreNull || Name != null) retVal.Add("Name", Name);
            if(!ignoreNull || DefaultAddress != null) retVal.Add("DefaultAddress", DefaultAddress);
            if(returnRole && (!ignoreNull || Role != null)) retVal.Add("Role", Role);

            return retVal;
        }


    }
}
