using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;

namespace ServerRA_AspnetCore.Model.Orders
{
    [FirestoreData]
    public class BasketEntryModel
    {
        [FirestoreProperty]
        [Required]
        public string? ProductId { get; set; }

        [FirestoreProperty]
        [Required]
        public int Count { get; set; }
    }
}
