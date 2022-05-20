using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerRA_AspnetCore.Model.General;
using ServerRA_AspnetCore.Model.Orders;
using ServerRA_AspnetCore.Services.Client;
using ServerRA_AspnetCore.Services.Client.Orders;

namespace ServerRA_AspnetCore.Controllers
{
    [Authorize]
    public class OrderController : BaseControllerWithFunctionality
    {
        private readonly OrderService orsrv;

        public OrderController()
        {
            orsrv = OrderService.getInstace();
        }

        [Route("[controller]/get/{orderid}")]
        [HttpGet("{orderid}")]
        public async Task<IActionResult> GetOrder(string orderid)
        {
            var uid = await UserService.getUserIDByToken(getAuthToken());
            try
            {
                var result = orsrv.getOrderDetails(orderid, uid);
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

        [Route("[controller]/get")]
        [HttpGet]
        public async Task<IActionResult> GetOrder()
        {
            var uid = await UserService.getUserIDByToken(getAuthToken());
            var result = orsrv.getOrdersForUser(uid);
            if (result == null)
                return Ok("[]");
            else
                return Ok(result);
        }

        [Route("staff/[controller]/get")]
        [HttpGet]
        public async Task<IActionResult> GetInProcessOrder()
        {
            var usrSrv = UserService.getInstance();
            var uid = await UserService.getUserIDByToken(getAuthToken());
            if(await usrSrv.IsUserStaff(uid) || await usrSrv.IsUserManager(uid))
            {
                return Unauthorized("Need staff priviledges or higher to access this functionality");
            }
            var result = orsrv.getUnfinishedOrders();
            if (result == null)
                return Ok("[]");
            else
                return Ok(result);
        }

        [Route("staff/[controller]/changestate/{orderid}")]
        [HttpPost("{orderid}")]
        public async Task<IActionResult> ChangeState(string orderid, string? state)
        {
            var usrSrv = UserService.getInstance();
            var uid = await UserService.getUserIDByToken(getAuthToken());
            if (await usrSrv.IsUserStaff(uid) || await usrSrv.IsUserManager(uid))
            {
                return Unauthorized("Need staff priviledges or higher to access this functionality");
            }
            if (state == null)
                return StatusCode(StatusCodes.Status400BadRequest, "state field cannot be empty");
            var result = orsrv.changeState(orderid, state);
            if (result == null)
                return StatusCode(StatusCodes.Status500InternalServerError);
            else
                return Ok(result);
        }

        [Route("staff/[controller]/message/{orderid}")]
        [HttpPost("{orderid}")]
        public async Task<IActionResult> AddMessage(string orderid, HistoryModel? newMessage)
        {
            var usrSrv = UserService.getInstance();
            var uid = await UserService.getUserIDByToken(getAuthToken());
            if (await usrSrv.IsUserStaff(uid) || await usrSrv.IsUserManager(uid))
            {
                return Unauthorized("Need staff priviledges or higher to access this functionality");
            }
            if (newMessage == null || newMessage.message == null || newMessage.message.Equals(""))
                return StatusCode(StatusCodes.Status400BadRequest, "Message field cannot be empty");
            var result = orsrv.addMessageToHistory(orderid, uid, newMessage);
            if (result == null)
                return StatusCode(StatusCodes.Status500InternalServerError);
            else
                return Ok(result);
        }

        [Route("staff/[controller]/remove/{orderid}")]
        [HttpPost("{orderid}")]
        public async Task<IActionResult> AddMessage(string orderid, OrderComponentModel[] product)
        {
            var usrSrv = UserService.getInstance();
            var uid = await UserService.getUserIDByToken(getAuthToken());
            if (await usrSrv.IsUserStaff(uid) || await usrSrv.IsUserManager(uid))
            {
                return Unauthorized("Need staff priviledges or higher to access this functionality");
            }
            var result = orsrv.removeProduct(orderid, uid, product);
            if (result == null)
                return StatusCode(StatusCodes.Status500InternalServerError);
            else
                return Ok(result);
        }
    }
}
