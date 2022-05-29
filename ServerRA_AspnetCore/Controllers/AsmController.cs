using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerRA_AspnetCore.Model;
using ServerRA_AspnetCore.Model.Basket;
using ServerRA_AspnetCore.Model.General;
using ServerRA_AspnetCore.Model.Orders;
using ServerRA_AspnetCore.Services;
using ServerRA_AspnetCore.Services.Client;
using ServerRA_AspnetCore.Services.Client.Orders;
using System.Text.Json;

namespace ServerRA_AspnetCore.Controllers
{
    public class AsmController : BaseControllerWithFunctionality
    {
        [Route("[controller]/get/{orderid}")]
        [Route("staff/[controller]/get/{orderid}")]
        [HttpGet]
        public async Task<IActionResult> GetOrder(string orderid)
        {
            var uid = await UserService.getUserIDByToken(getAuthToken());
            try
            {
                OrderModel? result;
                if (await usrSrv.IsUserStaff(uid) || await usrSrv.IsUserManager(uid))
                    result = await asmSrv.getOrderDetails(orderid, null);
                else
                    result = await asmSrv.getOrderDetails(orderid, uid);

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
            var result = await asmSrv.getOrdersForUser(uid);
            if (result == null)
                return Ok("[]");
            else
                return Ok(result);
        }

        [Route("staff/[controller]/get")]
        [HttpGet]
        public async Task<IActionResult> GetInProcessOrder(string? filter = null)
        {
            var uid = await UserService.getUserIDByToken(getAuthToken());
            if (await isNotAutorized(uid))
            {
                return Unauthorized("Need staff priviledges or higher to access this functionality");
            }
            var result = await asmSrv.getUnfinishedOrders(filter);
            if (result == null)
                return Ok("[]");
            else
                return Ok(result);
        }

        [Route("staff/[controller]/{orderid}/awaitcomponents")]
        [HttpPost]
        public async Task<IActionResult> Dispace(string orderid)
        {
            var uid = await UserService.getUserIDByToken(getAuthToken());
            if (await isNotAutorized(uid))
            {
                return Unauthorized("Need staff priviledges or higher to access this functionality");
            }
            return await ChangeState_internal(orderid, uid, AssemblyService.state_awaitComponenets, OrderService.state_process);
        }

        [Route("staff/[controller]/{orderid}/process")]
        [HttpPost]
        public async Task<IActionResult> ProcessState(string orderid)
        {
            var uid = await UserService.getUserIDByToken(getAuthToken());
            if (await isNotAutorized(uid))
            {
                return Unauthorized("Need staff priviledges or higher to access this functionality");
            }
            return await ChangeState_internal(orderid, uid, OrderService.state_process, AssemblyService.state_awaitComponenets);
        }

        [Route("staff/[controller]/{orderid}/dispace")]
        [HttpPost]
        public async Task<IActionResult> DispaceState(string orderid)
        {
            var uid = await UserService.getUserIDByToken(getAuthToken());
            if (await isNotAutorized(uid))
            {
                return Unauthorized("Need staff priviledges or higher to access this functionality");
            }
            return await ChangeState_internal(orderid, uid, OrderService.state_dispaced, OrderService.state_process);
        }

        [Route("staff/[controller]/{orderid}/deliver")]
        [HttpPost]
        public async Task<IActionResult> DeliverState(string orderid)
        {
            var uid = await UserService.getUserIDByToken(getAuthToken());
            if (await isNotAutorized(uid))
            {
                return Unauthorized("Need staff priviledges or higher to access this functionality");
            }
            return await ChangeState_internal(orderid, uid, OrderService.state_delivered, OrderService.state_dispaced);
        }



        [Route("staff/[controller]/message/{orderid}")]
        [HttpPost]
        public async Task<IActionResult> AddMessage(string orderid, HistoryModel? newMessage)
        {
            var uid = await UserService.getUserIDByToken(getAuthToken());
            if (await isNotAutorized(uid))
            {
                return Unauthorized("Need staff priviledges or higher to access this functionality");
            }
            if (newMessage == null || newMessage.message == null || newMessage.message.Equals(""))
                return StatusCode(StatusCodes.Status400BadRequest, "Message field cannot be empty");
            var result = await asmSrv.addMessageToHistory(orderid, uid, newMessage);
            if (result == null)
                return StatusCode(StatusCodes.Status500InternalServerError);
            else
                return Ok(asmSrv.getOrderDetails(orderid, null));
        }

        [Route("staff/[controller]/remove/{orderid}")]
        [HttpPost("{orderid}")]
        public async Task<IActionResult> RemoveItems(string orderid, bool tostorage = false, [FromBody] OrderComponentModel[]? products = null)
        {
            if (products == null)
            {
                return BadRequest("No products were given");
            }
            var uid = await UserService.getUserIDByToken(getAuthToken());
            if (await isNotAutorized(uid))
            {
                return Unauthorized("Need staff priviledges or higher to access this functionality");
            }
            var result = await asmSrv.removeProduct(orderid, uid, products, tostorage);
            if (!result)
                return StatusCode(StatusCodes.Status500InternalServerError);
            else
                return Ok(asmSrv.getOrderDetails(orderid, null));
        }

        [Route("staff/[controller]/add/{orderid}")]
        [HttpPost]
        public async Task<IActionResult> AddItems(string orderid, [FromBody] BasketEntryModel[]? products = null)
        {
            if (products == null)
            {
                return BadRequest("No products were given");
            }
            var uid = await UserService.getUserIDByToken(getAuthToken());
            if (await isNotAutorized(uid))
            {
                return Unauthorized("Need staff priviledges or higher to access this functionality");
            }
            var result = await asmSrv.addProduct(orderid, uid, products);
            if (!result)
                return StatusCode(StatusCodes.Status500InternalServerError);
            else
                return Ok(asmSrv.getOrderDetails(orderid, null));
        }

        //other things which are not entry points

        private readonly AssemblyService asmSrv;
        private readonly UserService usrSrv;

        public AsmController()
        {
            asmSrv = AssemblyService.getInstance();
            usrSrv = UserService.getInstance();
        }

        protected async Task<bool> isNotAutorized(string uid)
        {
            return !await usrSrv.IsUserStaff(uid) && !await usrSrv.IsUserManager(uid);
        }

        private async Task<IActionResult> ChangeState_internal(string orderid, string uid, string newState, string requiredState)
        {
            try
            {
                var result = await asmSrv.changeStateWithPrecondition(orderid, newState, requiredState);
                if (result == null)
                    return StatusCode(StatusCodes.Status500InternalServerError);
                else
                {
                    HistoryModel message = new HistoryModel();
                    message.message = "Order with id " + orderid + " is not in process to be delivered";
                    message.state = OrderService.state_dispaced;
                    await asmSrv.addMessageToHistory(orderid, uid, message);
                    return Ok(result);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
