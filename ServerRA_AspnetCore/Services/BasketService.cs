using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using ServerRA_AspnetCore.Model.Orders;

namespace ServerRA_AspnetCore.Services
{
    public class BasketService
    {
        private static BasketService? _instance;

        public static BasketService getInstance()
        {
            if(_instance == null)
            {
                _instance = new BasketService();
            }
            return _instance;
        }

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
                var ar = result.GetValue<object[]>("Basket");
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
        public async Task<BasketEntryModel[]> getBasketForCurrentUser(string uid)
        {
            var usdData = firestoreRef.Collection("userData").Document(uid);
            try
            {
                var snapshoot = await usdData.GetSnapshotAsync();
                BasketExtendedEntryModel[] basket = snapshoot.GetValue<BasketExtendedEntryModel[]>("Basket");

                if(basket == null)
                    return Array.Empty<BasketExtendedEntryModel>(); //for reasons where Basket would become null or whaterver

                //complete the response with the name and availability

                return basket;
            } 
            catch(InvalidOperationException)
            {
                return Array.Empty<BasketExtendedEntryModel>();
            }
        }

        //sems to be allright
        public async Task<BasketEntryModel[]> addElementToBasket(string uid, BasketEntryModel Element)
        {
            var usdData = firestoreRef.Collection("userData").Document(uid);

            await usdData.UpdateAsync("Basket",FieldValue.ArrayUnion(Element));
            return await getBasketForCurrentUser(uid);
        }

        //remove all elements
        public async Task<BasketEntryModel[]> changeElementCountToBasket(string uid, int index, int newCount)
        {            
            var usdData = await firestoreRef.Collection("userData").Document(uid).GetSnapshotAsync();

            var arr = usdData.GetValue<BasketEntryModel[]>("Basket");

            arr[index].Count = newCount;

            await firestoreRef.Collection("userData").Document(uid).UpdateAsync("Basket", arr);
            return await getBasketForCurrentUser(uid);
        }

        //sems to work
        public async Task<BasketEntryModel[]> removeElementFromBasket(string uid, BasketEntryModel Element)
        {
            var usdData = firestoreRef.Collection("userData").Document(uid);

            await usdData.UpdateAsync("Basket", FieldValue.ArrayRemove(Element));
            return await getBasketForCurrentUser(uid);
        }

        //sems to work
        public async Task<BasketEntryModel[]> removeElementFromBasket(string uid, int index)
        {
            var usdData = await firestoreRef.Collection("userData").Document(uid).GetSnapshotAsync();


            var arr = usdData.GetValue<BasketEntryModel[]>("Basket");
            if (index < 0 || index > arr.Length)
                throw new Exception();

            await removeElementFromBasket(uid, arr[index]);
            return await getBasketForCurrentUser(uid);
        }

        public async Task<BasketEntryModel[]> emptyTheBasket(string uid)
        {
            var usdData = firestoreRef.Collection("userData").Document(uid);

            await usdData.UpdateAsync("Basket", null);
            return await getBasketForCurrentUser(uid);
        }

        public async Task<string?> ToOrder()
        {
            return null;
        }

        public async Task<string?> ToAssembly()
        {
            return null;
        }
    }
}
