using Microsoft.AspNetCore.Mvc;
using ServerRA_AspnetCore.XMLRPC.External;

namespace ServerRA_AspnetCore.Controllers
{
    public class IntraServerController : Controller
    {
        UserDataRpcServiceImp rpcSrv;

        public IntraServerController()
        {
            rpcSrv = new UserDataRpcServiceImp();
        }

        [Route("[controller]/getPrivileges")]
        [HttpGet]
        public string GetUserPrivileges(string token)
        {
            return rpcSrv.GetUserPrivileges(token);
        }

        [Route("[controller]/getId")]
        [HttpGet]
        public string GetUserID(string token)
        {
            return rpcSrv.GetUserID(token);
        }

        [Route("[controller]/isAdmin")]
        [HttpGet]
        public bool IsUserAdmin(string token)
        {
            return rpcSrv.IsUserAdmin(token);
        }

        [Route("[controller]/isManager")]
        [HttpGet]
        public bool IsUserManager(string token)
        {
            return rpcSrv.IsUserManager(token);
        }
    }
}
