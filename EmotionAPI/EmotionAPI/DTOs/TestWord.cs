using Google.Cloud.Firestore;

namespace EmotionAPI.DTOs
{
    public class TestWord
    {
        [FirestoreProperty]
        public string titulo { get; set; }

        [FirestoreProperty]
        public string Emoji { get; set; }

        [FirestoreProperty]
        public string emocao { get; set; }

        [FirestoreProperty]
        public string descricao { get; set; }

        [FirestoreProperty]
        public DateTime data { get; set; }
    }
}
