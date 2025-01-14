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
using static Google.Cloud.Firestore.V1.StructuredQuery.Types;

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
            
            var collectionReference = _firestoreDb.Collection("entries");//nome do banco
            var documentReference = collectionReference.Document();
            var dictionary = data.GetType()
                                         .GetProperties(BindingFlags.Public | BindingFlags.Instance)//chama os meus dados
                                         .ToDictionary(
                                             prop => prop.Name, //retorna o campo
                                             prop => prop.GetValue(data) //dado do campo
                           );

            //adiciona o id do usuario
            if (HttpContext.Items["User"] is FirebaseToken decodedToken)
            {
                dictionary.Add("userId", decodedToken.Uid);
            }
            //adiciona a data de registro
            dictionary.Add("data", Google.Cloud.Firestore.Timestamp.GetCurrentTimestamp());

            await documentReference.SetAsync(dictionary);

            return Ok(new { documentId = documentReference.Id });
        }
    }
}

