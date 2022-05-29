using EdjCase.JsonRpc.Router;
using ServerRA_AspnetCore.Enternal;
using ServerRA_AspnetCore.Model.Basket;
using ServerRA_AspnetCore.Services.Client;

namespace ServerRA_AspnetCore.External.jsonRpc
{
    public class ServerComunicationController : RpcController
    {
        UserService usrSrv;

        public ServerComunicationController()
        {
            usrSrv = UserService.getInstance();
        }

        public async Task<string> GetUserPrivileges(string token)
        {
            return await usrSrv.getCrtUserRole(token);
        }

        public async Task<string> GetUserID(string token)
        {
            return await UserService.getUserIDByToken(token);
        }

        public async Task<bool> IsUserAdmin(string token)
        {
            return await usrSrv.IsUserAdmin(token);
        }

        public async Task<bool> IsUserManager(string token)
        {
            return await usrSrv.IsUserManager(token);
        }
    }
}
