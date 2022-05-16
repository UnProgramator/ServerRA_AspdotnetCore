using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace ServerRA_AspnetCore.Controllers
{
    public class BaseControllerWithFunctionality : ControllerBase
    {
        public string getAuthToken()
        {
            var token = this.Request.Headers[HeaderNames.Authorization][0];

            return token.Substring(token.IndexOf(' ') + 1);
        }
    }
}
