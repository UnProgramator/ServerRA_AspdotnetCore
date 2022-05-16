using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using ServerRA_AspnetCore.Exceptions;
using ServerRA_AspnetCore.Model.User;
using ServerRA_AspnetCore.Services;

namespace ServerRA_AspnetCore.Controllers
{
    [Authorize]
    public class UserController : BaseControllerWithFunctionality
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
            await usrSrv.signupUser(userData);

            //login the user
            var authLink = await usrSrv.signinUser(userData);

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
            var authLink = await usrSrv.signinUser(userData);

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
            var uid = await UserService.getUserIDByToken(getAuthToken());
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
            var uid = await UserService.getUserIDByToken(getAuthToken());
            var writen = await usrSrv.UpdateClient(uid, userData);

            return Ok();
        }
    }
}
