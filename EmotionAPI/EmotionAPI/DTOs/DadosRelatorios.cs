using Google.Cloud.Firestore;

namespace EmotionAPI.DTOs
{
    public class DadosRelatorios
    {

        [FirestoreProperty]
        public string Data_Inicio { get; set; }

        [FirestoreProperty]
        public string Data_Fim { get; set; }

        [FirestoreProperty]
        public string Nome { get; set; }
    }
}
