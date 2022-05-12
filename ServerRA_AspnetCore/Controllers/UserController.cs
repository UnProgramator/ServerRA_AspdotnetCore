using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using ServerRA_AspnetCore.Exceptions;
using ServerRA_AspnetCore.Model.User;
using ServerRA_AspnetCore.Services;

namespace ServerRA_AspnetCore.Controllers
{
    [Authorize]
    public class UserController : ControllerBase
    {
        private UserService usrSrv;

        public UserController()
        {
            usrSrv = UserService.getInstance();
        }

        [Route("[controller]/[action]")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserSignupModel userData)
        {
            //create new user
            await usrSrv.SignupUser(userData.getUserPublicInfo(), userData.Password);

            //login the user
            var authLink = await usrSrv.LoginUser(userData);

            if (authLink != null)
            {
                return Ok(authLink);
            }
            else
            {
                return StatusCode(StatusCodes.Status406NotAcceptable);
            }
        }

        [Route("[controller]/login")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserAuthenticationModel userData)
        {
            var authLink = await usrSrv.LoginUser(userData);

            if (authLink != null)
            {
                return Ok(authLink);
            }
            else
            {
                return StatusCode(StatusCodes.Status406NotAcceptable);
            }
        }

        //[Route("[controller]/logout")]
        //[HttpPost]
        //public async Task<IActionResult> Logout()
        //{

        //    return Ok(userData.LocalId);
        //}

        [Route("[controller]/get")]
        [HttpGet]
        public async Task<IActionResult> GenInfo()
        {
            var uid = await usrSrv.getCrtUserID(this);
            var requestedUserData = await usrSrv.getUserData(uid);
            if(requestedUserData != null)
            {
                return Ok(requestedUserData);
            }
            else
            {
                return StatusCode(StatusCodes.Status406NotAcceptable, "User not found. Requested user id is not a valid user id." + uid);
            }
        }

        [Route("[controller]/update")]
        [HttpPost]
        public async Task<IActionResult> UpdateInfo(UserInternalModel userData)
        {
            var usrId = await usrSrv.getCrtUserID(this);
            return Ok();
        }
    }
}
