using Google.Cloud.Firestore;
using ServerRA_AspnetCore.Model.General;
using ServerRA_AspnetCore.Model.User;

namespace ServerRA_AspnetCore.Model.Orders
{
    [FirestoreData]
    public class OrderModel: OrderResumeModel
    {
        [FirestoreProperty]
        public OrderComponentModel[]? content { get; set; }

        [FirestoreProperty]
        public OrderDetailsModel? details { get; set; }

        [FirestoreProperty]
        public HistoryModel[]? hystory { get; set; } = new HistoryModel[0];
    }
}
