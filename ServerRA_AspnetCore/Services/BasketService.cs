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

        private UserService usrSrv;
        private FirestoreDb firestoreRef;

        private BasketService()
        {
            usrSrv = UserService.getInstance();
            firestoreRef = FirebaseAccess.getFirestoreClient();
        }

        public async Task<int> getBasketCount(ControllerBase context)
        {
            string uid = await usrSrv.getCrtUserID(context);

            try
            {
                var result = await firestoreRef.Collection("userData").Document(uid).GetSnapshotAsync();
                return result.GetValue<object[]>("Basket").Length;
            } 
            catch (InvalidOperationException)
            {
                return 0;
            }
        }

        public async Task<BasketEntryModel[]> getBasketForCurrentUser(ControllerBase context)
        {
            string uid = await usrSrv.getCrtUserID(context);

            var usdData = firestoreRef.Collection("userData").Document(uid);
            try
            {
                var snapshoot = await usdData.GetSnapshotAsync();
                BasketEntryModel[] basket = snapshoot.GetValue<BasketExtendedEntryModel[]>("Basket");

                //complete the response with the name and availability

                return basket;
            } 
            catch(InvalidOperationException)
            {
                return Array.Empty<BasketEntryModel>();
            }
        }

        public async void addElementToBasket(ControllerBase context, BasketEntryModel Element)
        {
            string uid = await usrSrv.getCrtUserID(context);
            var usdData = firestoreRef.Collection("userData").Document(uid);

            await usdData.UpdateAsync("Basket",FieldValue.ArrayUnion(Element));
        }

        public async void changeElementCountToBasket(ControllerBase context, int position, int newCount)
        {
            string uid = await usrSrv.getCrtUserID(context);
            var usdData = firestoreRef.Collection("userData").Document(uid);

            await usdData.UpdateAsync("Basket." + position + ".Count", newCount);
        }

        public async void removeElementFromBasket(ControllerBase context, BasketEntryModel Element)
        {
            string uid = await usrSrv.getCrtUserID(context);
            var usdData = firestoreRef.Collection("userData").Document(uid);

            await usdData.UpdateAsync("Basket", FieldValue.ArrayRemove(Element));
        }
    }
}
