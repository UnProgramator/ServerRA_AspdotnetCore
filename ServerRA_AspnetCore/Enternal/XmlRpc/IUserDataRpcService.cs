using Horizon.XmlRpc.Core;

namespace ServerRA_AspnetCore.External.XMLRPC
{
    public interface IUserDataRpcService
    {
        [XmlRpcMethod("ServerRA_AspnetCore.getUserPrivileges")]
        public abstract string GetUserPrivileges(string token);


        [XmlRpcMethod("ServerRA_AspnetCore.getUserPrivileges")]
        public string GetUserID(string token);

        [XmlRpcMethod("ServerRA_AspnetCore.isUserAdmin")]
        public bool IsUserAdmin(string token);

        [XmlRpcMethod("ServerRA_AspnetCore.isUserManager")]
        public bool IsUserManager(string token);
    }
}
