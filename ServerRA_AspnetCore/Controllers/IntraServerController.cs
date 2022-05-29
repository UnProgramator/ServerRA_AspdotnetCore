using Microsoft.AspNetCore.Mvc;
using ServerRA_AspnetCore.Services.Client;

namespace ServerRA_AspnetCore.Controllers
{
    public class IntraServerController : Controller
    {
        UserService rpcSrv;

        public IntraServerController()
        {
            rpcSrv = UserService.getInstance();
        }

        [Route("[controller]/getPrivileges")]
        [HttpGet]
        public async Task<string> GetUserPrivileges(string token)
        {
            return await rpcSrv.getCrtUserRole(token);
        }

        [Route("[controller]/getId")]
        [HttpGet]
        public async Task<string> GetUserID(string token)
        {
            return await UserService.getUserIDByToken(token);
        }

        [Route("[controller]/isAdmin")]
        [HttpGet]
        public async Task<bool> IsUserAdmin(string token)
        {
            return await rpcSrv.IsUserAdmin(token);
        }

        [Route("[controller]/isManager")]
        [HttpGet]
        public async Task<bool> IsUserManager(string token)
        {
            return await rpcSrv.IsUserManager(token);
        }
    }
}
