using Google.Cloud.Firestore;

namespace ServerRA_AspnetCore.Model.Orders
{
    [FirestoreData]
    public class OrderResumeModel
    {
        [FirestoreDocumentId]
        public string? orderID { get; set; }

        [FirestoreProperty]
        public DateTime orderDate { get; set; }

        [FirestoreProperty]
        public float value { get; set; }

        [FirestoreProperty]
        public string? state { get; set; }

        public OrderResumeModel(string orderID, DateTime orderDate, float value, string state)
        {
            this.orderID = orderID;
            this.orderDate = orderDate;
            this.value = value;
            this.state = state;
        }

        public OrderResumeModel() { }
    }
}
