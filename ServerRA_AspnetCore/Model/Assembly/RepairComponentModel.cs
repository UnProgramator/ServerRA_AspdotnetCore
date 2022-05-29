using Google.Cloud.Firestore;

namespace ServerRA_AspnetCore.Model.Orders
{
    [FirestoreData]
    public class RepairComponentModel
    {
        [FirestoreProperty]
        public string? componentId { get; set; }

        [FirestoreProperty]
        public string? componentName { get; set; }

        [FirestoreProperty]
        public int quantity { get; set; }

        [FirestoreProperty]
        public float totalPrice { get; set; } 
    }
}
