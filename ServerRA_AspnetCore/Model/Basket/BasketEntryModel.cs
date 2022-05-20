using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;

namespace ServerRA_AspnetCore.Model.Basket
{
    [FirestoreData]
    public class BasketEntryModel
    {
        [FirestoreProperty]
        [Required]
        public string? productId { get; set; }

        [FirestoreProperty]
        [Required]
        public int count { get; set; }
    }
}
