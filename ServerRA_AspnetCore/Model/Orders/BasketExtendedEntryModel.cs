using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;

namespace ServerRA_AspnetCore.Model.Orders
{
    [FirestoreData]
    public class BasketExtendedEntryModel: BasketEntryModel
    {
        [FirestoreProperty]
        [Required]
        public string Name { get; set; }

        [FirestoreProperty]
        [Required]
        public string IsAvailable { get; set; }

        public BasketExtendedEntryModel()
        {
            Name = "default";
            IsAvailable = "No";
        }
    }
}
