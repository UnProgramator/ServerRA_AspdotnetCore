using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerRA_AspnetCore.Model;
using ServerRA_AspnetCore.Services;

using System.Text.Json;

namespace ServerRA_AspnetCore.Controllers
{
    public class AssemblingController : Controller
    {
        //public async bool createAssembleOrder(string userID, components?, float basePrice)
        //{

        //}

        [Route("[controller]/orders")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> getAssemblyOrdersByUserToken(string? token)
        {
            //var firebaseDB = FirebaseAccess.getFirebaseClient();

            //string userId = UserService.getUserIdForToken(token);

            //var order_list = await firebaseDB.GetAsync("userData/" + userId + "/Orders");

            //if(order_list == null)
            //    return NotFound();

            //var decoded_order_list = order_list.ResultAs<string[]>();

            //string response = "[";

            //foreach (var code in decoded_order_list)
            //{
            //    var dbResponse = firebaseDB.Get("assemblyOrders/" + code).Body;
            //    var rep = JsonSerializer.Deserialize<AssemblyFull>(dbResponse);
            //}
            //response = response.Substring(0, response.Length-1) + "]";

            //return Ok(response);
            return Ok();
        }
    }
}
