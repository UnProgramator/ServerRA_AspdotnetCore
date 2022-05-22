using Google.Cloud.Firestore;
using ServerRA_AspnetCore.Model.General;
using ServerRA_AspnetCore.Model.Orders;
using ServerRA_AspnetCore.Services.Extern;

namespace ServerRA_AspnetCore.Services.Client.Orders
{
    public class AssemblyService : OrderService
    {
        private static AssemblyService? _instance;

        public static AssemblyService getInstance()
        {
            if( _instance == null )
                _instance = new AssemblyService();
            return _instance;
        }

        public const string state_process = "Processing";
        public const string state_dispaced = "Dispaced";
        public const string state_delivered = "Delivered";

        private AssemblyService() : base("assembly")
        {
        }

        protected override HistoryModel genInitialMessage(string orderId)
        {
            HistoryModel model = new HistoryModel();

            model.sourceName = "Order Server";
            model.message = "Request for assemble created with number " + orderId;
            model.state = "Created";

            return model;
        }
    }
}
