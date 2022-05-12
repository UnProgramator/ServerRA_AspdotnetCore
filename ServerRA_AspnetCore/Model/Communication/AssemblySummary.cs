using System.ComponentModel.DataAnnotations;

namespace ServerRA_AspnetCore.Model
{
    public class AssemblySummary
    {
        [Required]
        DateTime date { get; set; }
        [Required]
        string state { get; set; }

        float? price { get; set; }

    }
}
