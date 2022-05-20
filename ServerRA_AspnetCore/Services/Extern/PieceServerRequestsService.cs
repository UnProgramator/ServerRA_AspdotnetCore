using ServerRA_AspnetCore.Model.Basket;

namespace ServerRA_AspnetCore.Services.Extern
{
    public class PieceServerRequestsService
    {
        private static PieceServerRequestsService? _instance;

        public static PieceServerRequestsService getInstance()
        {
            if (_instance == null)
            {
                _instance = new PieceServerRequestsService();
            }
            return _instance;
        }

        private PieceServerRequestsService() { }

        public BasketExtendedEntryModel[] getDetails(BasketEntryModel[] entries)
        {
            return new BasketExtendedEntryModel[0];
        }

        public bool validateOrder(BasketEntryModel[] entries)
        {
            return true;
        }

        public bool validateAssembly(BasketEntryModel[] entries)
        {
            return validateOrder(entries);
        }
    }
}
