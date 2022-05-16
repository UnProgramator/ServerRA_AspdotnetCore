using System.ComponentModel.DataAnnotations;

namespace ServerRA_AspnetCore.Model
{
    public class AssemblyFull
    {
        string customer { get; set; }

        DateTime date { get; set; }

        string state { get; set; }

        float? price { get; set; }

        Dictionary<string, int> components { get; set; }

        //DetailModel[] details;
    }
}
