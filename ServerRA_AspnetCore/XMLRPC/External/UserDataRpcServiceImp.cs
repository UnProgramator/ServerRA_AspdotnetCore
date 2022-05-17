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

        public string GetUserPrivileges(string token)
        {
            string id = UserService.getUserIDByToken(token).Result;
            return usrSrv.getUserRoleById(id).Result;
        }

        public string GetUserID(string token) => UserService.getUserIDByToken(token).Result;

        public bool IsUserAdmin(string token)
        {
            string id = UserService.getUserIDByToken(token).Result;
            return usrSrv.getUserRoleById(id).Result.Equals("Admin");
        }

        public bool IsUserManager(string token)
        {
            string id = UserService.getUserIDByToken(token).Result;
            return usrSrv.getUserRoleById(id).Result.Equals("Manager");
        }
    }
}
