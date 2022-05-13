using Firebase.Auth;
using FireSharp.Interfaces;
using ServerRA_AspnetCore.Model.User;

using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;

namespace ServerRA_AspnetCore.Services
{
    public class UserService
    {
        private static UserService? _instance = null;

        public static UserService getInstance()
        {
            if (_instance == null)
                _instance = new UserService();
            return _instance;
        }

        private IFirebaseClient firebaseRef;

        private UserService()
        {
            firebaseRef = FirebaseAccess.getFirebaseClient();
        }

        public async Task<bool> SignupUser(UserInternalModel userData, string password)
        {
            var authPrv = FirebaseAccess.getFirebaseAuthProvider();

            var result = await authPrv.CreateUserWithEmailAndPasswordAsync(userData.Email, password);

            userData.Role = "User";

            var response = await firebaseRef.SetAsync("userData/" + result.User.LocalId, userData);

            return true;
        }

        public async Task<FirebaseAuthLink?> LoginUser(UserAuthenticationModel userData)
        {
            FirebaseAuthLink authLink = await FirebaseAccess.getFirebaseAuthProvider().SignInWithEmailAndPasswordAsync(userData.Email, userData.Password);

            if(authLink.FirebaseToken == null)
                return null;
            else
                return authLink;
        }

        public async Task<UserInternalModel?> getUserData(string userId)
        {
            var response = await firebaseRef.GetAsync("userData/" + userId);
            return response.ResultAs<UserInternalModel>();
        }

        public async Task<string> getCrtUserID(ControllerBase context)
        {
            var token = context.Request.Headers[HeaderNames.Authorization][0];

            token = token.Substring(token.IndexOf(' ') + 1);

            var userData = await FirebaseAccess.getFirebaseAuthProvider().GetUserAsync(token);

            return userData.LocalId;
        }

        public async Task<string> getCrtUserRole(ControllerBase context)
        {
            var id = await getCrtUserID(context);
            if(id == null)
            {
                throw new Exception("No user loged in");
            }

            var data = await getUserData(id);
            if(data == null)
            {
                throw new Exception("Database error: invalid logged user or corupted data");
            }

            return data.Role;
        }

        public bool CrtUserHasRole(ControllerBase context, string rol)
        {
            return getCrtUserRole(context).Equals(rol);
        }

        public async Task<bool> UpdateClient(ControllerBase context, UserInternalModel newData) 
        {
            string uid = await getCrtUserID(context);

            var oldData = await firebaseRef.GetAsync("userData/" + uid);
            
            newData.complete(oldData.ResultAs<UserInternalModel>());

            var response = await firebaseRef.UpdateAsync("userData/" + uid, newData);

            return newData.Equals(response);
        }

    }
}
