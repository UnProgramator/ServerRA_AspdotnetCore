using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerRA_AspnetCore.Model.Basket;
using ServerRA_AspnetCore.Services.Client;

namespace ServerRA_AspnetCore.Controllers
{
    [Authorize]
    public class BasketController : BaseControllerWithFunctionality
    {
        private BasketService bskSrv;

        public BasketController()
        {
            bskSrv = BasketService.getInstance();
        }

        [Route("[controller]/size")]
        [HttpGet]
        public async Task<IActionResult> GetBasketCount()
        {
            var uid = await UserService.getUserIDByToken(getAuthToken());
            int result = await bskSrv.getBasketCount(uid);
            return Ok(result);
        }

        [Route("[controller]/get")]
        [HttpGet]
        public async Task<IActionResult> GetBasket()
        {
            var uid = await UserService.getUserIDByToken(getAuthToken());
            var result = await bskSrv.getBasketForCurrentUser(uid); ;
            return Ok(result);
        }

        [Route("[controller]/add")]
        [HttpPost]
        public async Task<IActionResult> AddItemToBakset(BasketEntryModel item)
        {
            var uid = await UserService.getUserIDByToken(getAuthToken());
            await bskSrv.addElementToBasket(uid, item);
            return Ok();
        }

        //doesn't write field corectly
        [Route("[controller]/update/{index}")]
        [HttpPost("{index}")]
        public async Task<IActionResult> UpdateCountToBakset(int index, int newCount)
        {
            try
            {
                var uid = await UserService.getUserIDByToken(getAuthToken());
                if (newCount > 0)
                {
                    return Ok(await bskSrv.changeElementCountToBasket(uid, index, newCount));
                }
                else if (newCount <= 0)
                {
                    var result = await bskSrv.removeElementFromBasket(uid, index);
                    if (result != null)
                        return Ok(result);
                }

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }

        //doesn't work, cant read field
        [Route("[controller]/remove/{index}")]
        [HttpPost("{index}")]
        public async Task<IActionResult> RemoveItemFromBakset(int index)
        {
            var uid = await UserService.getUserIDByToken(getAuthToken());
            var value = await bskSrv.removeElementFromBasket(uid, index);
            if(value != null)
                return base.Ok(value);
            return StatusCode(StatusCodes.Status500InternalServerError); //index out of bound
        }

        //ok
        [Route("[controller]/remove")]
        [HttpPost]
        public async Task<IActionResult> RemoveItemFromBakset(BasketExtendedEntryModel item)
        {
            var uid = await UserService.getUserIDByToken(getAuthToken());
            return Ok(await bskSrv.removeElementFromBasket(uid, item));
        }

        [Route("[controller]/empty")]
        [HttpPost]
        public async Task<IActionResult> EmptyTheBakset()
        {
            var uid = await UserService.getUserIDByToken(getAuthToken());
            return Ok(await bskSrv.emptyTheBasket(uid));
        }

        [Route("[controller]/order")]
        public async Task<IActionResult> BasketToOrderAsync()
        {
            var uid = await UserService.getUserIDByToken(getAuthToken());
            var result = await bskSrv.ToOrder(uid);
            return Ok(result);
        }

        [Route("[controller]/assemble")]
        public async Task<IActionResult> BasketToAssembleAsync()
        {
            var uid = await UserService.getUserIDByToken(getAuthToken());
            var result = await bskSrv.ToAssembly(uid);
            return Ok(result);
        }
    }
}
