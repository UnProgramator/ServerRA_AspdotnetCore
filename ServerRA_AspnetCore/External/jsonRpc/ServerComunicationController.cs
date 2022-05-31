using EdjCase.JsonRpc.Router;
using ServerRA_AspnetCore.Enternal;
using ServerRA_AspnetCore.Model.Basket;
using ServerRA_AspnetCore.Services.Client;

namespace ServerRA_AspnetCore.External.jsonRpc
{
    [RpcRoute("server")]
    public class ServerComunicationController : RpcController
    {
        UserService usrSrv;

        public ServerComunicationController()
        {
            usrSrv = UserService.getInstance();
        }

        public async Task<string> getUserPrivileges(string token)
        {
            return await usrSrv.getCrtUserRole(await getUserID(token));
        }

        public async Task<string> getUserID(string token)
        {
            return await UserService.getUserIDByToken(token);
        }

        public async Task<bool> isUserAdmin(string token)
        {
            return await usrSrv.IsUserAdmin(await getUserID(token));
        }

        public async Task<bool> isUserManager(string token)
        {
            return await usrSrv.IsUserManager(await getUserID(token));
        }

        public async Task<bool> isUserStaff(string token)
        {
            return await usrSrv.IsUserStaff(await getUserID(token));
        }
    }
}
