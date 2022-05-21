using Google.Cloud.Firestore;

namespace ServerRA_AspnetCore.Model.Orders
{
    [FirestoreData]
    public class OrderInternalModel: OrderModel
    {
        [FirestoreProperty]
        public string userId { get; private set; }

        public OrderInternalModel(string userId)
        {
            this.userId = userId;
        }

        public OrderInternalModel()
        {
            this.userId = "not set";
        }
    }
}
