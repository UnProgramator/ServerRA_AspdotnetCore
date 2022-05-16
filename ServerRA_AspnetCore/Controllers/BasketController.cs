using Microsoft.AspNetCore.Mvc;
using ServerRA_AspnetCore.Model.Orders;
using ServerRA_AspnetCore.Services;

namespace ServerRA_AspnetCore.Controllers
{
    [Route("[controller]/[action]")]
    public class BasketController : Controller
    {
        private BasketService bskSrv;

        public BasketController()
        {
            bskSrv = BasketService.getInstance();
        }

        [HttpGet]
        public async Task<IActionResult> GetBasketCount()
        {
            int result = await bskSrv.getBasketCount(this); ;
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetBasket()
        {
            var result = await bskSrv.getBasketForCurrentUser(this); ;
            return Ok(result);
        }

        [HttpPost]
        public IActionResult AddItemToBakset(BasketEntryModel item)
        {
            bskSrv.addElementToBasket(this, item);
            return Ok();
        }

        [HttpPost("{index}")]
        public IActionResult RemoveItemToBakset(int index)
        {
            //kSrv.removeElementFromBasket(this, index);
            return Ok();
        }
    }
}
