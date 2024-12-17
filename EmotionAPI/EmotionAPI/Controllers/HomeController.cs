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

namespace EmotionAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly FirestoreDb _firestoreDb;

        public HomeController(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
        }


        [HttpGet("GetAllDocuments")]


        public async Task<IActionResult> GetAllDocuments()
        {
            // Verifica se o token decodificado está presente no contexto
            if (HttpContext.Items["User"] is not FirebaseToken decodedToken)
            {
                return Unauthorized(new { Message = "User not authenticated." });
            }

            // Referência à coleção no Firestore
            var collectionReference = _firestoreDb.Collection("entries");
            var snapshot = await collectionReference.GetSnapshotAsync();

            var userDocuments = new List<Dictionary<string, object>>();

            foreach (var document in snapshot.Documents)
            {
                var data = document.ToDictionary();

                // Verifica se a entrada possui o campo "userId" e se ele corresponde ao UID do usuário
                if (data.TryGetValue("userId", out var userId) && userId?.ToString() == decodedToken.Uid)
                {
                    // Processa o campo "data" se ele existir
                    if (data.TryGetValue("data", out var dataField))
                    {
                        string dateTimeString = null;

                        // Se o campo for do tipo Firestore Timestamp
                        if (dataField is Timestamp firestoreTimestamp)
                        {
                            // Converte o Firestore Timestamp para DateTime
                            var dateTimeValue = firestoreTimestamp.ToDateTime();

                            // Formata o DateTime como string (exemplo: "yyyy-MM-dd HH:mm:ss")
                            dateTimeString = dateTimeValue.ToString("dd-MM-yyyy");
                        }

                        // Substitui o valor no dicionário se a conversão foi realizada
                        if (dateTimeString != null)
                        {
                            data["data"] = dateTimeString;
                        }
                    }

                    userDocuments.Add(data);
                }
            }

            return Ok(userDocuments);
        }




        [HttpPost("AddDocument")]
        public async Task<IActionResult> AddDocument([FromBody] TestWord data)
        {
            var collectionReference = _firestoreDb.Collection("entries");
            var documentReference = collectionReference.Document();
            var dictionary = data.GetType()
                                         .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                         .ToDictionary(
                                             prop => prop.Name,
                                             prop => prop.GetValue(data)
                           );

            if (HttpContext.Items["User"] is FirebaseToken decodedToken)
            {
                dictionary.Add("userId", decodedToken.Uid);
            }
            dictionary.Add("data", Google.Cloud.Firestore.Timestamp.GetCurrentTimestamp());

            // Save the data (including the emoji) to Firestore
            await documentReference.SetAsync(dictionary);

            return Ok(new { documentId = documentReference.Id });
        }
    }
}

