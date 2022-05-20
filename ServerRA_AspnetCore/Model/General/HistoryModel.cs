using Google.Cloud.Firestore;

namespace ServerRA_AspnetCore.Model.General
{
    [FirestoreData]
    public class HistoryModel
    {
        [FirestoreProperty]
        public string? sourceName { get; set; }

        [FirestoreProperty]
        public DateTime date { get; set; }

        [FirestoreProperty]
        public string? message { get; set; }

        [FirestoreProperty]
        public string state { get; set; } = "-";
    }
}
