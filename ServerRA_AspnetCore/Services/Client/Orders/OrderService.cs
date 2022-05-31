using Google.Cloud.Firestore;
using ServerRA_AspnetCore.Enternal;
using ServerRA_AspnetCore.Model.Basket;
using ServerRA_AspnetCore.Model.General;
using ServerRA_AspnetCore.Model.Orders;
using ServerRA_AspnetCore.Model.User;
using ServerRA_AspnetCore.Services.Extern;

namespace ServerRA_AspnetCore.Services.Client.Orders
{
    public class OrderService
    {
        private static OrderService? _instance;

        private static object _locker = new object();

        public static OrderService getInstance()
        {
            if(_instance == null)
                lock(_locker)
                    if (_instance == null)
                        _instance = new OrderService("orders");
            return _instance;
        }

        public const string state_process = "Processing";
        public const string state_dispaced = "Dispaced";
        public const string state_delivered = "Delivered";

        protected virtual string getOrderStartingState() => state_process;


        protected static DateTime getCrtTime() => DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

        protected string collectionName;

        protected FirestoreDb fdb;
        protected UserService usrSrv;

        protected OrderService(string CollectionName) {
            fdb = FirebaseAccess.getFirestoreClient();
            usrSrv = UserService.getInstance();
            collectionName = CollectionName;
        }

        public virtual async Task<string?> addNewOrder(string uid, BasketEntryModel[] componenets)
        {
            int i = 0;
            float value = 0;

            if (!await ServerCommunication.getInstance().areAllAvailable(componenets))
                throw new Exception("insuficient parts available");

            BasketExtendedEntryModel[] componentsExt = await ServerCommunication.getInstance().getAvailability(componenets);

            if (componentsExt == null)
                return null;
            if (componentsExt.Length == 0)
            {
                throw new BadHttpRequestException("Basket is empty");
            }

            var components = new OrderComponentModel[componentsExt.Length];
            foreach (var entry in componentsExt)
            {
                components[i] = new OrderComponentModel();

                components[i].componentId = entry.productId;
                components[i].componentName = entry.name;
                components[i].quantity = entry.count;
                components[i].totalPrice = entry.price * entry.count;

                value += components[i].totalPrice;
                i++;
            }

            OrderInternalModel om = new OrderInternalModel(uid);

            om.orderDate = getCrtTime();
            om.content = components;
            om.state = getOrderStartingState();
            om.value = value;
            om.history = Array.Empty<HistoryModel>();

            var data = await usrSrv.getUserData(uid);

            om.details = new OrderDetailsModel();
            om.details.clinetName = data?.name;
            om.details.address = data?.defaultAddress;

            var response = await fdb.Collection(collectionName).AddAsync(om);

            string docId = response.Id;

            var initMsg = genInitialMessage(docId);

            initMsg.date = om.orderDate;

            insertMessageToHystory(docId, initMsg);

            return docId;
        }

        protected virtual HistoryModel genInitialMessage(string orderId)
        {
            HistoryModel hst = new HistoryModel();
            hst.sourceName = "Order Server";
            hst.message = "Command with number " + orderId + " created";
            hst.state = "Created";

            return hst;
        }

        public virtual async Task<OrderResumeModel[]?> getOrdersForUser(string uid)
        {
            OrderResumeModel[]? result = null;

            var response = fdb.Collection(collectionName).WhereEqualTo("userId", uid);

            var snap = await response.GetSnapshotAsync();

            int size = snap.Count;

            if(size == 0)
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

        public virtual async Task<OrderModel?> getOrderDetails(string oid, string? uid)
        {
            OrderModel? result = null;

            var response = fdb.Collection(collectionName).Document(oid);

            var snap = await response.GetSnapshotAsync();

            if (uid != null && !snap.GetValue<string>("userId").Equals(uid))
                throw new AccessViolationException("Order does not belong to current user");

            if (!snap.Exists)
            {
                throw new DataMisalignedException("Command " + oid + " not found"); // I was lazy to define a new exception
            }

            result = snap.ConvertTo<OrderModel>();

            return result;
        }

        public virtual async Task<OrderResumeModel[]?> getUnfinishedOrders()
        {
            OrderResumeModel[]? result = null;

            var response = fdb.Collection(collectionName).WhereEqualTo("state", state_process).OrderBy("orderDate");

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

        public async Task<OrderModel?> changeStateWithPrecondition(string oid, string newState, string requiredState)
        {
            var snap = await fdb.Collection(collectionName).Document(oid).GetSnapshotAsync();

            var oldState = snap.GetValue<string>("state");

            if (oldState.Equals(requiredState))
                return await changeState(oid, newState);
            else
                throw new Exception("Requested chnage is invalid, cannot change state from " + oldState + " to " + newState);
        }

        public virtual async Task<OrderModel?> changeState(string oid, string newState)
        {
            var response = await fdb.Collection(collectionName).Document(oid).UpdateAsync("state", newState);

            if(response != null)
                return await getOrderDetails(oid, null);
            else
                return null;
        }

        public async Task<OrderModel?> addMessageToHistory(string oid, string uid, HistoryModel message)
        {
            message.date = getCrtTime();
            message.sourceName = (await usrSrv.getUserData(uid))?.name;
            if (message.sourceName == null)
                return null;
            insertMessageToHystory(oid, message);

            return await getOrderDetails(oid, null);
        }

        protected WriteResult insertMessageToHystory(string oid, HistoryModel message)
        {
            return fdb.Collection(collectionName).Document(oid).UpdateAsync("history", FieldValue.ArrayUnion(message)).Result;
        }

        protected async Task<double> verifyItem(string oid, OrderComponentModel[] components, HistoryModel removeMsg)
        {
            double sum = 0;

            var result = await fdb.Collection(collectionName).Document(oid).GetSnapshotAsync();

            var vec = result.GetValue<OrderComponentModel[]>("content");

            if (vec == null)
                throw new Exception("InternalServerError");

            foreach (var comp in components)
                foreach(var item in vec)
                {
                    if (item == null) continue;
                    if (item.componentId.Equals(comp.componentId) && item.quantity.Equals(comp.quantity) && item.componentName.Equals(comp.componentName))
                    {
                        if (item.totalPrice.Equals(comp.totalPrice))
                        {
                            removeMsg.message += "Removed item " + item.componentName + ", code " + item.componentId + " from order.\n";
                            sum += comp.totalPrice;
                            break;
                        }
                        else
                        {
                            throw new InvalidDataException("Price do not corespond to stored price. Fraud allert");
                            //if price is higer, it induce the idea that the customer hoped "to trick"
                            //and get a bigger refund then the actual money he paid for components 
                        }
                    }
                }

            return sum;
        }

        public virtual async Task<bool> removeProduct(string oid, string uid, OrderComponentModel[] product, bool returnToBasket)
        {
            //verify price and update for every item
              

            HistoryModel removeMsg = new HistoryModel();

            removeMsg.state = "Update";

            double val = await verifyItem(oid, product, removeMsg);        

            var result = await fdb.Collection(collectionName).Document(oid).UpdateAsync("content", FieldValue.ArrayRemove(product));

            await fdb.Collection(collectionName).Document(oid).UpdateAsync("value", FieldValue.Increment(-val));

            await addMessageToHistory(oid, uid, removeMsg);

            return true;
        }
    }
}
