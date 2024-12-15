using EmotionAPI.DTOs; // Import do modelo EmotionData
using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Database;
using Firebase.Database.Query;
using FirebaseAdmin.Auth;
using System.Threading.Tasks;

namespace EmotionAPI.Services
{
    public class FirebaseService
    {
        private readonly FirebaseAuthProvider _firebaseAuthProvider;
        public User? current => null;
        public FirebaseService(FirebaseAuthProvider firebaseAuthProvider)
        {
            // URL do Realtime Database
            _firebaseAuthProvider = firebaseAuthProvider;
        }


        public async Task<string> CreateUser(string username, string password)
        {
            var User = new UserRecordArgs
            {
                Email = username,
                Password = password
            };
            var user = await FirebaseAuth.DefaultInstance.CreateUserAsync(User);
            return user.Uid;
        }

       /* public async Task<bool> SaveEmotionToFirebase(TestWord emotionData)
        {
            try
            {
                var reference = await _firebaseClient
                    .Child("emotions")
                    .PostAsync(emotionData);

                if (reference != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar no Firebase: {ex.Message}");
                return false;
            }
        }*/

    }
}
