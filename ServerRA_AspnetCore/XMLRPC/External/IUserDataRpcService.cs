using Horizon.XmlRpc.Core;

namespace ServerRA_AspnetCore.XMLRPC.External
{
    public interface IUserDataRpcService
    {
        [XmlRpcMethod("UserData.getUserPrivileges")]
        public string GetUserPrivileges(string token);


        [XmlRpcMethod("UserData.getUserPrivileges")]
        public string GetUserID(string token);

        [XmlRpcMethod("UserData.isUserAdmin")]
        public bool IsUserAdmin(string token);

        [XmlRpcMethod("UserData.isUserManager")]
        public bool IsUserManager(string token);
    }
}
