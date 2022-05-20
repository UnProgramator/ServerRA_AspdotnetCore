using ServerRA_AspnetCore.Enternal.TempHTTPReq;
using ServerRA_AspnetCore.Model.Basket;
using ServerRA_AspnetCore.Model.IntraServer;

namespace ServerRA_AspnetCore.Enternal
{
    public interface ServerCommunication
    {
        private static ServerCommunication? _instance;
        public static ServerCommunication getInstance()
        {
            if (_instance == null) { 
                _instance = new HttpServerComunication();
            }
            return _instance;
        }

        BasketExtendedEntryModel[] getAvailability(BasketEntryModel[] items);
        bool areAllAvailable(BasketEntryModel[] items);
    }
}
