using Firebase.Auth;
using FireSharp;
using FireSharp.Interfaces;

namespace ServerRA_AspnetCore.Services
{
    public class FirebaseAccess
    {
        public const string database_url = "jIvVHLmeBCPtpBcmx9yGtxHBeMH2";
        public const string web_api_key = "AIzaSyD4L5mu4RnToo3JL-Hz3L_UzR-AuwyVgKI";
        public const string app_key = "1:449726311356:web:6b06b1126f37de8975d9b9";

        private static IFirebaseClient? _client = null;
        private static FirebaseAuthProvider? _authPrv = null;

        public static IFirebaseClient getFirebaseClient()
        {
            if(_client == null){
                IFirebaseConfig config = new FireSharp.Config.FirebaseConfig
                {
                    AuthSecret = "oHkvoe4sBkwhcNIuh7DNmCDoxdT19hCTeYASAlUR",
                    BasePath = "https://computercompany-64270-default-rtdb.europe-west1.firebasedatabase.app/"
                };
                _client = new FirebaseClient(config);
            }
            return _client;
        }

        public static FirebaseAuthProvider getFirebaseAuthProvider()
        {
            if (_authPrv == null)
            {
                _authPrv = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig(web_api_key));
            }
            return _authPrv;
        }
        
    }
}
