using EmotionAPI.DTOs;
using EmotionAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Firebase.Auth.Providers;
using FirebaseAdmin.Auth;
using System.Security.Claims;
using Google.Cloud.Firestore;
using System.Text.Json;
using System.Reflection;
using Firebase.Auth;
using Google.Cloud.Language.V1;

namespace EmotionAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly FirestoreDb _firestoreDb;
        private readonly LanguageServiceClient _languageServiceClient;

        public HomeController(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
            _languageServiceClient = LanguageServiceClient.Create();
        }


        [HttpPost("GetAllDocuments")]


        public async Task<IActionResult> GetAllDocuments([FromBody] FilterDTO filter)
        {
            // Verifica se o token existe
            if (HttpContext.Items["User"] is not FirebaseToken decodedToken)
            {
                return Unauthorized(new { Message = "User not authenticated." });
            }

            DateTime startDateTime = (DateTime)(filter.startDate != null ? filter.startDate : DateTime.MinValue.ToUniversalTime());
            DateTime endDateTime = (DateTime)(filter.endDate != null ? filter.endDate : DateTime.MaxValue.ToUniversalTime());
            var collectionReference = _firestoreDb.Collection("entries").WhereGreaterThanOrEqualTo("data", Timestamp.FromDateTime(startDateTime)).WhereLessThanOrEqualTo("data", Timestamp.FromDateTime(endDateTime));
            var snapshot = await collectionReference.GetSnapshotAsync();

            var userDocuments = new List<Dictionary<string, object>>();

            foreach (var document in snapshot.Documents)
            {
                var data = document.ToDictionary();

                // Verifica o userID
                if (data.TryGetValue("userId", out var userId) && userId?.ToString() == decodedToken.Uid)
                {
                    if (data.TryGetValue("data", out var dataField))
                    {
                        string? dateTimeString = null;

                        if (dataField is Timestamp firestoreTimestamp)
                        {
                            // Converte Timestamp para DateTime
                            var dateTimeValue = firestoreTimestamp.ToDateTime();
                            dateTimeString = dateTimeValue.ToString("dd-MM-yyyy");
                        }

                        // Substitui o valor no dicionário se a conversão foi realizada
                        if (dateTimeString != null)
                        {
                            data["data"] = dateTimeString;
                        }
                        userDocuments.Add(data);
                    }
                }
            }

            return Ok(userDocuments);
        }




        [HttpPost("AddDocument")]
        public async Task<IActionResult> AddDocument([FromBody] DadosRegistros data)
        {
            if (string.IsNullOrEmpty(data.descricao))
            {
                return BadRequest(new { Message = "O texto não pode estar vazio." });
            }

            try
            {
                var document = Document.FromPlainText(data.descricao);
                var sentiment = await _languageServiceClient.AnalyzeSentimentAsync(document);

                float score = sentiment.DocumentSentiment.Score;
                float magnitude = sentiment.DocumentSentiment.Magnitude;

                var collectionReference = _firestoreDb.Collection("entries");
                var documentReference = collectionReference.Document();

                var dictionary = data.GetType()
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .ToDictionary(
                        prop => prop.Name,
                        prop => prop.GetValue(data)
                    );

                // Adiciona o ID do usuário
                if (HttpContext.Items["User"] is FirebaseToken decodedToken)
                {
                    dictionary.Add("userId", decodedToken.Uid);
                }

                // Adiciona a data de registro
                dictionary.Add("data", Google.Cloud.Firestore.Timestamp.GetCurrentTimestamp());

                // Converte os valores antes de salvar
                dictionary.Add("score", Math.Round(Convert.ToDouble(score), 1));
                dictionary.Add("magnitude", Math.Round(Convert.ToDouble(magnitude), 1));

                
                try
                {
                    await documentReference.SetAsync(dictionary);
                    Console.WriteLine("Documento salvo com sucesso!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao salvar no Firestore: {ex.Message}");
                    return StatusCode(500, new { Message = "Erro ao salvar os dados no Firestore." });
                }

                return Ok(new { documentId = documentReference.Id, score, magnitude });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro geral: {ex.Message}");
                return StatusCode(500, new { Message = "Erro interno ao processar a solicitação." });
            }
        }

    }
}

