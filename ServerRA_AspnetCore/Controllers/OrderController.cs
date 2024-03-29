﻿using Microsoft.AspNetCore.Authorization;
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
        private readonly OrderService ordSrv;
        private readonly UserService usrSrv;

        public OrderController()
        {
            ordSrv = OrderService.getInstance();
            usrSrv = UserService.getInstance();
        }

        protected async Task<bool> isNotAutorized(string uid)
        {
            return !await usrSrv.IsUserStaff(uid) && !await usrSrv.IsUserManager(uid);
        }

        [Route("[controller]/get/{orderid}")]
        [Route("staff/[controller]/get/{orderid}")]
        [HttpGet("{orderid}")]
        public async Task<IActionResult> GetOrder(string orderid)
        {
            var uid = await UserService.getUserIDByToken(getAuthToken());
            try
            {
                OrderModel? result;
                if (await usrSrv.IsUserStaff(uid) || await usrSrv.IsUserManager(uid))
                    result = await ordSrv.getOrderDetails(orderid, null);
                else
                    result = await ordSrv.getOrderDetails(orderid, uid);

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
            var result = await ordSrv.getOrdersForUser(uid);
            if (result == null)
                return Ok("[]");
            else
                return Ok(result);
        }

        [Route("staff/[controller]/get")]
        [HttpGet]
        public async Task<IActionResult> GetInProcessOrder()
        {
            var uid = await UserService.getUserIDByToken(getAuthToken());
            if(await isNotAutorized(uid))
            {
                return Unauthorized("Need staff priviledges or higher to access this functionality");
            }
            var result = await ordSrv.getUnfinishedOrders();
            if (result == null)
                return Ok("[]");
            else
                return Ok(result);
        }

        [Route("staff/[controller]/changestate/{orderid}")]
        [HttpPost("{orderid}")]
        public async Task<IActionResult> ChangeState(string orderid, string? state)
        {
            var uid = await UserService.getUserIDByToken(getAuthToken());
            if (await isNotAutorized(uid))
            {
                return Unauthorized("Need staff priviledges or higher to access this functionality");
            }
            if (state == null)
                return StatusCode(StatusCodes.Status400BadRequest, "state field cannot be empty");
            var result = await ordSrv.changeState(orderid, state);
            if (result == null)
                return StatusCode(StatusCodes.Status500InternalServerError);
            else
                return Ok(result);
        }

        [Route("staff/[controller]/{orderid}/dispace")]
        [HttpPost]
        public async Task<IActionResult> Dispace(string orderid)
        {
            var uid = await UserService.getUserIDByToken(getAuthToken());
            if (await isNotAutorized(uid))
            {
                return Unauthorized("Need staff priviledges or higher to access this functionality");
            }
            try { 
                var result = await ordSrv.changeStateWithPrecondition(orderid, OrderService.state_dispaced, OrderService.state_process);
                if (result == null)
                    return StatusCode(StatusCodes.Status500InternalServerError);
                else
                {
                    HistoryModel message = new HistoryModel();
                    message.message = "Order with id " + orderid + " is not in process to be delivered";
                    message.state = OrderService.state_dispaced;
                    await ordSrv.addMessageToHistory(orderid, uid, message);
                    return Ok(result);
                }
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("staff/[controller]/{orderid}/deliver")]
        [HttpPost]
        public async Task<IActionResult> Deliver(string orderid)
        {
            var uid = await UserService.getUserIDByToken(getAuthToken());
            if (await isNotAutorized(uid))
            {
                return Unauthorized("Need staff priviledges or higher to access this functionality");
            }
            try
            {
                var result = await ordSrv.changeStateWithPrecondition(orderid, OrderService.state_delivered, OrderService.state_dispaced);
                if (result == null)
                    return StatusCode(StatusCodes.Status500InternalServerError);
                else
                {
                    HistoryModel message = new HistoryModel();
                    message.message = "Order with id " + orderid + " has been delivered";
                    message.state = OrderService.state_delivered;
                    await ordSrv.addMessageToHistory(orderid, uid, message);
                    return Ok(result);
                }
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
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
            var result = await ordSrv.addMessageToHistory(orderid, uid, newMessage);
            if (result == null)
                return StatusCode(StatusCodes.Status500InternalServerError);
            else
                return Ok(ordSrv.getOrderDetails(orderid, null));
        }

        [Route("staff/[controller]/remove/{orderid}")]
        [HttpPost("{orderid}")]
        public async Task<IActionResult> RemoveItems(string orderid, bool tostorage = false, [FromBody] OrderComponentModel[]? products = null)
        {
            if(products == null)
            {
                return BadRequest("No products were given");
            }
            var uid = await UserService.getUserIDByToken(getAuthToken());
            if (await isNotAutorized(uid))
            {
                return Unauthorized("Need staff priviledges or higher to access this functionality");
            }
            var result = await ordSrv.removeProduct(orderid, uid, products, tostorage);
            if (!result)
                return StatusCode(StatusCodes.Status500InternalServerError);
            else
                return Ok(ordSrv.getOrderDetails(orderid, null));
        }
    }
}
