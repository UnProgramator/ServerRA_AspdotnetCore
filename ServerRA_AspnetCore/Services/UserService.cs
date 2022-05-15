using Firebase.Auth;
using FireSharp.Interfaces;
using ServerRA_AspnetCore.Model.User;

using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Firestore;

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

        private FirestoreDb firestoreRef;

        private UserService()
        {
            firestoreRef = FirebaseAccess.getFirestoreClient();
        }

        public async Task<bool> SignupUser(UserInternalModel userData, string password)
        {
            var authPrv = FirebaseAccess.getFirebaseAuthProvider();

            var result = await authPrv.CreateUserWithEmailAndPasswordAsync(userData.Email, password);

            userData.Role = "User";

            //var response = await firestoreRef.SetAsync("userData/" + result.User.LocalId, userData);

            var response = await firestoreRef.Collection("userData").Document(result.User.LocalId).CreateAsync(userData);

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
            var response = await firestoreRef.Collection("userData").Document(userId).GetSnapshotAsync();
            return response?.ConvertTo<UserInternalModel>();
        }

        public async Task<string> getUserIDByToken(string token)
        {
            var userData = await FirebaseAccess.getFirebaseAuthProvider().GetUserAsync(token);

            return userData.LocalId;
        }

        public async Task<string> getUserMailByToken(string token)
        {
            var userData = await FirebaseAccess.getFirebaseAuthProvider().GetUserAsync(token);

            return userData.Email;
        }

        public async Task<string> getCrtUserID(ControllerBase context)
        {
            var token = context.Request.Headers[HeaderNames.Authorization][0];

            token = token.Substring(token.IndexOf(' ') + 1);

            return await getUserIDByToken(token);
        }

        public async Task<string> getUserRoleById(string id)
        {
            var data = await getUserData(id);
            if (data == null)
            {
                throw new Exception("Database error: invalid logged user or corupted data");
            }

            return data.Role;
        }

        public async Task<string> getCrtUserRole(ControllerBase context)
        {
            var id = await getCrtUserID(context);
            if(id == null)
            {
                throw new Exception("No user loged in");
            }

            return await getUserRoleById(id);
        }

        public bool CrtUserHasRole(ControllerBase context, string rol)
        {
            return getCrtUserRole(context).Equals(rol);
        }

        public async Task<bool> UpdateClient(ControllerBase context, UserInternalModel newData) 
        {
            string uid = await getCrtUserID(context);

            var oldData = await firestoreRef.Collection("userData").Document(uid).GetSnapshotAsync();

            var response = await firestoreRef.Collection("userData").Document(uid).UpdateAsync(newData.getAsDict());

            return newData.Equals(response);
        }

    }
}
