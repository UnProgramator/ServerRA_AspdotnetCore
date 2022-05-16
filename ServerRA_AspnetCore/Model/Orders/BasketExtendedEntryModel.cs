using Google.Cloud.Firestore;

namespace ServerRA_AspnetCore.Model.Orders
{
    [FirestoreData]
    public class BasketExtendedEntryModel: BasketEntryModel
    {
        [FirestoreProperty]
        string Name { get; set; } = "default";

        [FirestoreProperty]
        string IsAvailable { get; set; } = "default";
    }
}
