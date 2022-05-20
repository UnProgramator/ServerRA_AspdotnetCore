using ServerRA_AspnetCore.Model.Basket;
using ServerRA_AspnetCore.Model.IntraServer;

namespace ServerRA_AspnetCore.Enternal.TempHTTPReq
{
    public class HttpServerComunication : ServerCommunication
    {

        public bool areAllAvailable(BasketEntryModel[] items)
        {
            bool response = true;


            return response;
        }

        public BasketExtendedEntryModel[] getAvailability(BasketEntryModel[] items)
        {
            throw new NotImplementedException();
        }

    }
}
