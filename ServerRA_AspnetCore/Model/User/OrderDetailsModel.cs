using Google.Cloud.Firestore;

namespace ServerRA_AspnetCore.Model.User
{
    [FirestoreData]
    public class OrderDetailsModel
    {
        [FirestoreProperty]
        public string? clinetName { get; set; }

        [FirestoreProperty]
        public string? address { get; set; }
    }
}
