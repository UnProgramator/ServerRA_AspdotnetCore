using Horizon.XmlRpc.AspNetCore;
using ServerRA_AspnetCore.Services;

namespace ServerRA_AspnetCore.XMLRPC.External
{
    public class UserDataRpcServiceImp: XmlRpcService, IUserDataRpcService
    {
        UserService usrSrv;

        public UserDataRpcServiceImp()
        {
            usrSrv = UserService.getInstance();
        }

        string IUserDataRpcService.GetUserPrivileges(string token)
        {
            string id = UserService.getUserIDByToken(token).Result;
            return usrSrv.getUserRoleById(id).Result;
        }

        string IUserDataRpcService.GetUserID(string token) => UserService.getUserIDByToken(token).Result;

        bool IUserDataRpcService.IsUserAdmin(string token)
        {
            string id = UserService.getUserIDByToken(token).Result;
            return usrSrv.getUserRoleById(id).Result.Equals("Admin");
        }

        bool IUserDataRpcService.IsUserManager(string token)
        {
            string id = UserService.getUserIDByToken(token).Result;
            return usrSrv.getUserRoleById(id).Result.Equals("Manager");
        }
    }
}
