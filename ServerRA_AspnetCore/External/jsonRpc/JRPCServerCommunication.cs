using EdjCase.JsonRpc.Client;
using EdjCase.JsonRpc.Core;
using ServerRA_AspnetCore.Enternal;
using ServerRA_AspnetCore.Model.Basket;

namespace ServerRA_AspnetCore.External.jsonRpc
{
    public class JRPCServerCommunication : ServerCommunication
    {
        private string url = "https://server-parts-ada.herokuapp.com/api";

        private object lock_id_gen = new object();

        private int _id = 0;

        private string getId()
        {
            lock (lock_id_gen)
            {
                return "ID" + _id++;
            }   
        }

        public async Task<bool> areAllAvailable(BasketEntryModel[] items)
        {
            List<List<object>> lst = new List<List<object>>(items.Length);

            foreach (var item in items)
            {
                lst.Add(new List<object> { item.productId, item.count });
            }

            try
            {
                RpcClient cli = new RpcClient(new Uri(url));
                RpcRequest req = RpcRequest.WithParameterList("areComponentsAvailable", new[] { lst }, getId());
                RpcResponse<bool> resp = await cli.SendRequestAsync<bool>(req);
                return resp.Result;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<BasketExtendedEntryModel[]> getAvailability(BasketEntryModel[] items)
        {
            List<List<object>> lst = new List<List<object>>(items.Length);

            foreach (var item in items)
            {
                lst.Add(new List<object> { item.productId, item.count });
            }

            try
            {
                RpcClient cli = new RpcClient(new Uri(url));
                RpcRequest req = RpcRequest.WithParameterList("getComponentsInfo", new[] { lst }, "id2");
                RpcResponse<BasketExtendedEntryModel[]> resp = await cli.SendRequestAsync<BasketExtendedEntryModel[]>(req);
                return resp.Result;
            }
            catch (Exception)
            {
                return Array.Empty<BasketExtendedEntryModel>();
            }
        }

        public async Task<bool> returnProducts(BasketEntryModel[] items)
        {
            List<List<object>> lst = new List<List<object>>(items.Length);

            foreach (var item in items)
            {
                lst.Add(new List<object> { item.productId, item.count });
            }
            try
            {
                RpcClient cli = new RpcClient(new Uri(url));
                RpcRequest req = RpcRequest.WithParameterList("returnComponents", new[] { lst }, "id2");
                RpcResponse<BasketExtendedEntryModel[]> resp = await cli.SendRequestAsync<BasketExtendedEntryModel[]>(req);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
