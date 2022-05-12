using System.ComponentModel.DataAnnotations;

namespace ServerRA_AspnetCore.Model
{
    public class OrderIdModel
    {
        [Required]
        public string oid { get; set; }
    }
}
