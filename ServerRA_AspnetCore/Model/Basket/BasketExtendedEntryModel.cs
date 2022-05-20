using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;

namespace ServerRA_AspnetCore.Model.Basket
{
    [FirestoreData]
    public class BasketExtendedEntryModel : BasketEntryModel
    {
        [FirestoreProperty]
        [Required]
        public string name { get; set; }

        [FirestoreProperty]
        [Required]
        public string isAvailable { get; set; }

        [FirestoreProperty]
        [Required]
        public float price { get; set; }

        public BasketExtendedEntryModel()
        {
            name = "default";
            isAvailable = "No";
        }
    }
}
