using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerRA_AspnetCore.Model;
using ServerRA_AspnetCore.Services;
using ServerRA_AspnetCore.Services.Client;
using ServerRA_AspnetCore.Services.Client.Orders;
using System.Text.Json;

namespace ServerRA_AspnetCore.Controllers
{
    public class AssemblingController : BaseControllerWithFunctionality
    {
        private readonly AssemblyService orsrv;
        private readonly UserService usrSrv;

        public AssemblingController()
        {
            orsrv = AssemblyService.getInstace();
            usrSrv = UserService.getInstance();
        }

        protected async Task<bool> isNotAutorized(string uid)
        {
            return !await usrSrv.IsUserStaff(uid) && !await usrSrv.IsUserManager(uid);
        }

        [Route("[controller]/get/{orderid}")]
        [HttpGet("{orderid}")]
        public async Task<IActionResult> GetOrder(string orderid)
        {
            var uid = await UserService.getUserIDByToken(getAuthToken());
            try
            {
                var result = await orsrv.getOrderDetails(orderid, uid);
                if (result == null)
                    return BadRequest(result);
                else
                    return Ok(result);
            }
            catch (DataMisalignedException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
            catch (AccessViolationException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
