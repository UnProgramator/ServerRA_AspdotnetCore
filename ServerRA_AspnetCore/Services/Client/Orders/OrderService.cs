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

        public static OrderService getInstace()
        {
            if(_instance == null)
                _instance = new OrderService();
            return _instance;
        }

        public const string state_process = "Processing";
        public const string state_dispaced = "Dispaced";
        public const string state_delivered = "Delivered";


        private static DateTime getCrtTime() => DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

        public const string collectionName = "orders";

        private FirestoreDb fdb;
        private UserService usrSrv;

        private OrderService() {
            fdb = FirebaseAccess.getFirestoreClient();
            usrSrv = UserService.getInstance();
        }

        public async Task<string?> addNewOrder(string uid, BasketEntryModel[] componenets)
        {
            int i = 0;
            float value = 0;

            if (!ServerCommunication.getInstance().areAllAvailable(componenets))
                throw new Exception("insuficient parts available");

            BasketExtendedEntryModel[] componentsExt = ServerCommunication.getInstance().getAvailability(componenets);

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
            om.state = state_process;
            om.value = value;
            om.history = Array.Empty<HistoryModel>();

            var data = await usrSrv.getUserData(uid);

            om.details = new OrderDetailsModel();
            om.details.clinetName = data?.name;
            om.details.address = data?.defaultAddress;

            var response = await fdb.Collection(collectionName).AddAsync(om);

            string docId = response.Id;

            HistoryModel hst = new HistoryModel();
            hst.date = om.orderDate;
            hst.sourceName = "Order Server";
            hst.message = "Command with number " + docId + " created";
            hst.state = "Created";

            var response2 = await insertMessageToHystory(docId, hst);

            return docId;
        }

        public async Task<OrderResumeModel[]?> getOrdersForUser(string uid)
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

        public async Task<OrderModel?> getOrderDetails(string oid, string? uid)
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

        public async Task<OrderResumeModel[]?> getUnfinishedOrders()
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

        public async Task<OrderModel?> changeState(string oid, string newState)
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
            await insertMessageToHystory(oid, message);

            return await getOrderDetails(oid, null);
        }

        private async Task<object> insertMessageToHystory(string oid, HistoryModel message)
        {
            return await fdb.Collection(collectionName).Document(oid).UpdateAsync("history", FieldValue.ArrayUnion(message));
        }

        public async Task<bool> verifyItem(string oid, OrderComponentModel comp)
        {
            var result = await fdb.Collection(collectionName).Document(oid).GetSnapshotAsync();

            var vec = result.GetValue<OrderComponentModel[]>("content");

            if(vec == null)
                return false;

            foreach(var item in vec)
            {
                if (item == null) continue;
                if (item.componentId.Equals(comp.componentId) && item.quantity.Equals(comp.quantity) && item.componentName.Equals(comp.componentName))
                {
                    if (item.totalPrice.Equals(comp.totalPrice))
                    {
                        return true;
                    }
                    else
                    {
                        throw new Exception("Price do not corespond to stored price. Fraud allert");
                        //if price is higer, it induce the idea that the customer hoped "to trick"
                        //and get a bigger refund then the actual money he paid for components 
                    }
                }
            }

            return false;
        }

        public async Task<OrderModel?> removeProduct(string oid, string uid, OrderComponentModel[] product)
        {
            //verify price and update for every item
            foreach(var item in product)
                if(!await verifyItem(oid, item))
                {
                    return null;
                }

            foreach (var item in product)
            {
                var result = await fdb.Collection(collectionName).Document(oid).UpdateAsync("content", FieldValue.ArrayRemove(product));

                HistoryModel removeMsg = new HistoryModel();

                removeMsg.state = "Update";
                removeMsg.message = "Removed item " + item.componentName + ", code " + item.componentId + " from order.";

                await addMessageToHistory(oid, uid, removeMsg);
            }

            return await getOrderDetails(oid, null);
        }
    }
}
