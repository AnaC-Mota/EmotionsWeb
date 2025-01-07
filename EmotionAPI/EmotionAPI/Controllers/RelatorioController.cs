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
    private readonly ChatGptService _chatGptService;
    private readonly PdfService _pdfService;

    public RelatorioController(FirestoreDb firestoreDb,
                                ChatGptService chatGptService,
                                PdfService pdfService)
    {
        _firestoreDb = firestoreDb;
        _googleSheetsService = new GoogleSheetsService();
        _chatGptService = chatGptService;
        _pdfService = pdfService;
    }

    [HttpPost("Planilha")]
    public async Task<IActionResult> CriarRelatorio([FromBody] FilterDTO filter)
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

                // Verifica o userId
                if (data.TryGetValue("userId", out var userId) && userId?.ToString() == decodedToken.Uid)
                {
                    if (data.TryGetValue("data", out var dataField))
                    {
                        var dateTimeValue = new DateTime();
                        string dateTimeString = null;

                        if (dataField is Timestamp firestoreTimestamp)
                        {
                            // Converte Timestamp para DateTime
                            dateTimeValue = firestoreTimestamp.ToDateTime();
                            dateTimeString = dateTimeValue.ToString("dd-MM-yyyy");
                        }

                        // Substitui o valor no dicionário se a conversão foi realizada
                        if (dateTimeString != null)
                        {
                            data["data"] = dateTimeString;

                            if (filter.startDate.HasValue && DateTime.TryParse(dateTimeString, out var recordDate) && recordDate < filter.startDate.Value)
                            {
                                continue;
                            }

                            if (filter.endDate.HasValue && DateTime.TryParse(dateTimeString, out var recordDate2) && recordDate2 > filter.endDate.Value)
                            {
                                continue;
                            }
                        }

                        if(dateTimeValue >= filter.startDate && dateTimeValue <= filter.endDate)
                        {
                            userDocuments.Add(data);
                        }
                        
                    }

                }
            }

            // Converte os dados em formato de planilha
            List<IList<object>> sheetData = GoogleSheetsService.ConvertToSheetFormat(userDocuments);

            // Limpa os dados existentes na planilha
            GoogleSheetsService.ClearSheet();

            // Insere os novos dados na planilha
            GoogleSheetsService.WriteToSheet(sheetData);

            return Ok("Registros substituídos na planilha com sucesso!");
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao atualizar a planilha: {ex.Message}");
        }
    }

    [HttpPost("RelatorioAdicional")]
    public async Task<IActionResult> GerarRelatorioAdicional(DateTime? startDate, DateTime? endDate)
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
            int totalEntries = 0;

            foreach (var document in snapshot.Documents)
            {
                var data = document.ToDictionary();

                if (data.TryGetValue("userId", out var userId) && userId?.ToString() == decodedToken.Uid)
                {
                    if (data.TryGetValue("data", out var dataField))
                    {
                        string dateTimeString = null;

                        if (dataField is Timestamp firestoreTimestamp)
                        {
                            var dateTimeValue = firestoreTimestamp.ToDateTime();
                            dateTimeString = dateTimeValue.ToString("dd-MM-yyyy");
                        }

                        if (dateTimeString != null)
                        {
                            data["data"] = dateTimeString;

                            if (startDate.HasValue && DateTime.TryParse(dateTimeString, out var recordDate) && recordDate < startDate.Value)
                            {
                                continue;
                            }

                            if (endDate.HasValue && DateTime.TryParse(dateTimeString, out var recordDate2) && recordDate2 > endDate.Value)
                            {
                                continue;
                            }
                        }
                    }

                    userDocuments.Add(data);
                    totalEntries++;
                }
            }

            var resumoRelatorio = $"Relatório Adicional: Total de Entradas: {totalEntries}\n" +
                                  $"Data Início: {startDate?.ToString("dd-MM-yyyy") ?? "não especificada"}\n" +
                                  $"Data Fim: {endDate?.ToString("dd-MM-yyyy") ?? "não especificada"}\n\n" +
                                  "Detalhes das Entradas:\n";

            foreach (var doc in userDocuments)
            {
                resumoRelatorio += string.Join(", ", doc.Values) + "\n";
            }

            // Gerar relatório usando o ChatGPT
            string relatorioGerado = await _chatGptService.GerarRelatorio(resumoRelatorio);

            // Gerar o PDF com o relatório gerado
            byte[] pdfBytes = _pdfService.GerarPdf(relatorioGerado);

            // Retornar o PDF gerado
            return File(pdfBytes, "application/pdf", "relatorio_adicional.pdf");
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao gerar o relatório adicional: {ex.Message}");
        }
    }



}