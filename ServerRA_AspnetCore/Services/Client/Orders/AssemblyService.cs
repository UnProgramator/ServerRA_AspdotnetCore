using Google.Cloud.Firestore;
using ServerRA_AspnetCore.Enternal;
using ServerRA_AspnetCore.Model.Basket;
using ServerRA_AspnetCore.Model.General;
using ServerRA_AspnetCore.Model.Orders;
using ServerRA_AspnetCore.Services.Extern;

namespace ServerRA_AspnetCore.Services.Client.Orders
{
    public class AssemblyService : OrderService
    {
        private static AssemblyService? _instance;
        private static object _instanceLock = new object();

        public static new AssemblyService getInstance()
        {
            if (_instance == null)
                lock (_instanceLock)
                {
                    if (_instance == null)
                        _instance = new AssemblyService();
                }
            return _instance;
        }

        public const string state_awaitComponenets = "Await Components";
        protected override string getOrderStartingState() => state_awaitComponenets;

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

        internal async Task<OrderResumeModel[]?> getUnfinishedOrders(string? filter = null)
        {
            OrderResumeModel[]? result = null;

            var response = fdb.Collection(collectionName).OrderBy("orderDate");

            if(filter != null)
            {
                response = response.WhereEqualTo("state", filter);
            }
            else //the default displays only the procesable, both "await componenets" and "processing"
            {
                response = response.WhereIn("state", new string[] { state_process, state_awaitComponenets });
            }

            var snap = await response.GetSnapshotAsync();

            int size = snap.Count;

            if (size == 0)
            {
                return Array.Empty<OrderResumeModel>();
            }

            result = new OrderResumeModel[size];

            int i = 0;

            foreach (var doc in snap)
            {
                result[i] = doc.ConvertTo<OrderResumeModel>();
                i++;
            }

            return result;
        }

        internal async Task<bool> addProduct(string orderid, string uid, BasketEntryModel[] componenets)
        {
            var usdData = fdb.Collection(collectionName).Document(uid);

            if (!await ServerCommunication.getInstance().areAllAvailable(componenets))
                throw new Exception("insuficient parts available");

            BasketExtendedEntryModel[]? componentsExt = await ServerCommunication.getInstance().getAvailability(componenets);

            HistoryModel removeMsg = new HistoryModel();

            removeMsg.state = "Update";
            removeMsg.message = "";

            int i = 0;
            double value=0;
            var components = new OrderComponentModel[componentsExt.Length];
            foreach (var entry in componentsExt)
            {
                removeMsg.message += "Added product " + components[i].componentName + ", code " + components[i].componentId + " to assembly order";

                components[i] = new OrderComponentModel();

                components[i].componentId = entry.productId;
                components[i].componentName = entry.name;
                components[i].quantity = entry.count;
                components[i].totalPrice = entry.price * entry.count;

                value += components[i].totalPrice;
                i++;
            }

            await usdData.UpdateAsync("content", FieldValue.ArrayUnion(components));
            await usdData.UpdateAsync("value", FieldValue.Increment(value));
            await addMessageToHistory(orderid, uid, removeMsg);

            return true;
        }
    }
}
