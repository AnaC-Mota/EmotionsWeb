using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using EmotionAPI.DTOs;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Microsoft.Win32;
using FirebaseAdmin.Auth;

public class RelatorioController : Controller
{
    private readonly FirestoreDb _firestoreDb;
    private readonly GoogleSheetsService _googleSheetsService;

    public RelatorioController(FirestoreDb firestoreDb)
    {
        _firestoreDb = firestoreDb;
        _googleSheetsService = new GoogleSheetsService();
    }

    [HttpPost("Planilha")]
    public async Task<IActionResult> CriarRelatorio(DateTime? startDate, DateTime? endDate)
    {
        try
        {
            if (HttpContext.Items["User"] is not FirebaseToken decodedToken)
            {
                return Unauthorized(new { Message = "User not authenticated." });
            }
            var entriesRef = _firestoreDb.Collection("entries");
            var snapshot = await entriesRef.GetSnapshotAsync();

            var userDocuments = new List<Dictionary<string, object>>();

            foreach (var document in snapshot.Documents)
            {
                var data = document.ToDictionary();

                // Verifica o userID
                if (data.TryGetValue("userId", out var userId) && userId?.ToString() == decodedToken.Uid)
                {
                    if (data.TryGetValue("data", out var dataField))
                    {
                        string dateTimeString = null;

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

                            if (startDate.HasValue && DateTime.TryParse(dateTimeString, out var recordDate) && recordDate < startDate.Value)
                            {
                                continue; // Desconsidera registros anteriores à startDate
                            }

                            if (endDate.HasValue && DateTime.TryParse(dateTimeString, out var recordDate2) && recordDate2 > endDate.Value)
                            {
                                continue; // Desconsidera registros posteriores à endDate
                            }
                        }
                    }

                    userDocuments.Add(data);
                }
            }

            // Converte os dados 
            List<IList<object>> sheetData = GoogleSheetsService.ConvertToSheetFormat(userDocuments);

            foreach (var row in sheetData)
            {
                Console.WriteLine(string.Join(", ", row));  
            }

            // Insere os dados 
            GoogleSheetsService.WriteToSheet(sheetData);

            // Retorna uma resposta de sucesso
            return Ok("Relatório criado e dados inseridos na planilha com sucesso!");
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao criar relatório: {ex.Message}");
        }
    }

}