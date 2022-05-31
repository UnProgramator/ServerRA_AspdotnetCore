using ServerRA_AspnetCore.Enternal.TempHTTPReq;
using ServerRA_AspnetCore.External.jsonRpc;
using ServerRA_AspnetCore.Model.Basket;
using ServerRA_AspnetCore.Model.IntraServer;

namespace ServerRA_AspnetCore.Enternal
{
    public interface ServerCommunication
    {
        private static ServerCommunication? _instance;

        private static object _instanceLock = new object();
        public static ServerCommunication getInstance()
        {
            if (_instance == null) {
                lock (_instanceLock)
                {
                    if (_instance == null)
                    {
                        _instance = new JRPCServerCommunication();
                    }
                }
            }
            return _instance;
        }

        Task<BasketExtendedEntryModel[]> getAvailability(BasketEntryModel[] items);
        Task<bool> areAllAvailable(BasketEntryModel[] items);
    }
}
