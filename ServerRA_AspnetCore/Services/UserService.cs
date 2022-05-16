using Firebase.Auth;
//using FireSharp.Interfaces;
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

        public static async Task<string> getUserIDByToken(string token)
        {
            var userData = await FirebaseAccess.getFirebaseAuthProvider().GetUserAsync(token);

            return userData.LocalId;
        }

        private FirestoreDb firestoreRef;

        private UserService()
        {
            firestoreRef = FirebaseAccess.getFirestoreClient();
        }

        public async Task<bool> signupUser(UserSignupModel userData, string role = "User")
        {
            var authPrv = FirebaseAccess.getFirebaseAuthProvider();

            var result = await authPrv.CreateUserWithEmailAndPasswordAsync(userData.Email, userData.Password);

            //var response = await firestoreRef.SetAsync("userData/" + result.User.LocalId, userData);

            var publicData = userData.getUserPublicInfo();
            publicData.Role = role;

            var response = await firestoreRef.Collection("userData").Document(result.User.LocalId).CreateAsync(publicData);

            return true;
        }

        public async Task<FirebaseAuthLink?> signinUser(UserAuthenticationModel userData)
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

        

        public async Task<string> getUserMailByToken(string token)
        {
            var userData = await FirebaseAccess.getFirebaseAuthProvider().GetUserAsync(token);

            return userData.Email;
        }

        public async Task<string> getUserRoleById(string id)
        {
            var data = await getUserData(id);
            if (data != null)
            {
                return data.Role;
            }

            throw new Exception("Database error: invalid logged user or corupted data");
        }

        public async Task<string> getCrtUserRole(string? uid)
        {
            if(uid == null)
            {
                throw new Exception("No user loged in");
            }

            return await getUserRoleById(uid);
        }

        public bool CrtUserHasRole(string uid, string rol)
        {
            return getCrtUserRole(uid).Equals(rol);
        }

        public async Task<bool> UpdateClient(string uid, UserInternalModel newData) 
        {
            var oldData = await firestoreRef.Collection("userData").Document(uid).GetSnapshotAsync();

            var response = await firestoreRef.Collection("userData").Document(uid).UpdateAsync(newData.getAsDict());

            return newData.Equals(response);
        }

    }
}
