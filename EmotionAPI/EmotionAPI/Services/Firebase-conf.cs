using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace EmotionAPI.Services
{
    public class Firebase_conf
    {
        public static void InitializeFirebase()
        {
            if (FirebaseApp.DefaultInstance != null) return;

            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile("emocoes-4f9b5-firebase-adminsdk-dwqof-e589d28cf8.json") 
            });
        }
    }
}
