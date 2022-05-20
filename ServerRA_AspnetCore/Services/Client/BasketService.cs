using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using ServerRA_AspnetCore.Enternal;
using ServerRA_AspnetCore.Model.Basket;
using ServerRA_AspnetCore.Services.Client.Orders;
using ServerRA_AspnetCore.Services.Extern;

namespace ServerRA_AspnetCore.Services.Client
{
    public class BasketService
    {
        private static BasketService? _instance;

        public static BasketService getInstance()
        {
            if (_instance == null)
            {
                _instance = new BasketService();
            }
            return _instance;
        }

        private const string _basketField = "basket";
        private FirestoreDb firestoreRef;

        private BasketService()
        {
            firestoreRef = FirebaseAccess.getFirestoreClient();
        }

        //sems to be allright
        public async Task<int> getBasketCount(string uid)
        {
            try
            {
                var result = await firestoreRef.Collection("userData").Document(uid).GetSnapshotAsync();
                var ar = result.GetValue<object[]>(_basketField);
                if (ar != null)
                    return ar.Length;
                else
                    return 0; //if value is null or so, we get null pointer => 0 elements
            }
            catch (InvalidOperationException)
            {
                return 0; //when field not declared we get this exception
            }
        }

        //sems to be allright
        //bot yet fully implemented
        public async Task<BasketExtendedEntryModel[]> getBasketForCurrentUser(string uid)
        {
            var usdData = firestoreRef.Collection("userData").Document(uid);
            try
            {
                var snapshoot = await usdData.GetSnapshotAsync();
                BasketEntryModel[] basket = snapshoot.GetValue<BasketEntryModel[]>(_basketField);

                if (basket == null)
                    return Array.Empty<BasketExtendedEntryModel>(); //for reasons where Basket would become null or whaterver

                var response = ServerCommunication.getInstance().getAvailability(basket);

                if (response != null)
                    return response;
                else
                    return Array.Empty<BasketExtendedEntryModel>();
            }
            catch (InvalidOperationException)
            {
                return Array.Empty<BasketExtendedEntryModel>();
            }
        }

        //sems to be allright
        public async Task<BasketExtendedEntryModel[]> addElementToBasket(string uid, BasketEntryModel Element)
        {
            var usdData = firestoreRef.Collection("userData").Document(uid);

            await usdData.UpdateAsync(_basketField, FieldValue.ArrayUnion(Element));
            return await getBasketForCurrentUser(uid);
        }

        //remove all elements
        public async Task<BasketExtendedEntryModel[]> changeElementCountToBasket(string uid, int index, int newCount)
        {
            var usdData = await firestoreRef.Collection("userData").Document(uid).GetSnapshotAsync();

            var arr = usdData.GetValue<BasketEntryModel[]>(_basketField);

            arr[index].count = newCount;

            await firestoreRef.Collection("userData").Document(uid).UpdateAsync(_basketField, arr);
            return await getBasketForCurrentUser(uid);
        }

        //sems to work
        public async Task<BasketExtendedEntryModel[]> removeElementFromBasket(string uid, BasketEntryModel Element)
        {
            var usdData = firestoreRef.Collection("userData").Document(uid);

            await usdData.UpdateAsync(_basketField, FieldValue.ArrayRemove(Element));
            return await getBasketForCurrentUser(uid);
        }

        //sems to work
        public async Task<BasketExtendedEntryModel[]> removeElementFromBasket(string uid, int index)
        {
            var usdData = await firestoreRef.Collection("userData").Document(uid).GetSnapshotAsync();


            var arr = usdData.GetValue<BasketExtendedEntryModel[]>(_basketField);
            if (index < 0 || index > arr.Length)
                throw new Exception();

            await removeElementFromBasket(uid, arr[index]);
            return await getBasketForCurrentUser(uid);
        }

        public async Task<BasketExtendedEntryModel[]> emptyTheBasket(string uid)
        {
            var usdData = firestoreRef.Collection("userData").Document(uid);

            await usdData.UpdateAsync(_basketField, null);
            return await getBasketForCurrentUser(uid);
        }

        public async Task<string?> ToOrder(string uid)
        {
            var orderComponents = await this.getBasketForCurrentUser(uid);
            var passed = await OrderService.getInstace().addNewOrder(uid, orderComponents);

            if(passed != null)
            {
                await emptyTheBasket(uid);
            }

            return passed;
        }

        public async Task<string?> ToAssembly(string uid)
        {
            var orderComponents = await this.getBasketForCurrentUser(uid);
            string? passed = null; // await OrderService.getInstace().addNewOrder(uid, orderComponents);

            if (passed != null)
            {
                await emptyTheBasket(uid);
            }

            return passed;
        }
    }
}
