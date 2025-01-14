using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using EmotionAPI.DTOs;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Microsoft.Win32;
using FirebaseAdmin.Auth;
using Google.Protobuf.Reflection;
using Firebase.Auth;

public class RelatorioController : Controller
{
    private readonly FirestoreDb _firestoreDb;
    private readonly GoogleSheetsService _googleSheetsService;
    private readonly PdfService _pdfService;
    private readonly GraficoService _graficoService;


    public RelatorioController(FirestoreDb firestoreDb,
                                PdfService pdfService,
                                GraficoService graficoService)
    {
        _firestoreDb = firestoreDb;
        _googleSheetsService = new GoogleSheetsService();
        _pdfService = pdfService;
        _graficoService = graficoService;
    }

    [HttpPost("Grafico")]
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
            DateTime startDateTime = (DateTime)(filter.startDate != null ? filter.startDate : DateTime.MinValue.ToUniversalTime());
            DateTime endDateTime = (DateTime)(filter.endDate != null ? filter.endDate : DateTime.MaxValue.ToUniversalTime());
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

                        if (dateTimeValue >= filter.startDate && dateTimeValue <= filter.endDate)
                        {
                            userDocuments.Add(data);
                        }

                    }

                }
            }

            var contagemEmocoes = new Dictionary<string, int>();

            foreach (var document in userDocuments)
            {
                // Check if the emotion field exists in the document
                if (document.TryGetValue("emocao", out var emocao))
                {
                    if (emocao != null)
                    {
                        string emocaoValue = emocao.ToString();

                        if (contagemEmocoes.ContainsKey(emocaoValue))
                        {
                            // If the emotion exists, increment the count
                            contagemEmocoes[emocaoValue]++;
                        }
                        else
                        {
                            // If it doesn't exist, add it to the dictionary with a count of 1
                            contagemEmocoes[emocaoValue] = 1;
                        }
                    }
                }
            }

            string caminhoImagem = _graficoService.GerarGraficoDeEmocoes(contagemEmocoes);

            string pdfUrl = _pdfService.GerarPDFComImagem(caminhoImagem);

            string fullPdfUrl = $"{Request.Scheme}://{Request.Host}{pdfUrl}";

            var relatorioRef = _firestoreDb.Collection("relatorios").Document();

            // Criação do dicionário com os dados do relatório
            var dadosRelatorio = new Dictionary<string, object>
            {
            { "UserId", decodedToken.Uid },                 
            { "data_fim", Timestamp.FromDateTime(endDateTime) },
            { "data_inicio", Timestamp.FromDateTime(startDateTime) },
            { "data_reg", Timestamp.FromDateTime(DateTime.UtcNow) },
            { "nome", filter.Title != null ? filter.Title : "" }, 
            { "relatorio", fullPdfUrl } 
        };

            // Salvar o documento na coleção "relatorio"
            await relatorioRef.SetAsync(dadosRelatorio);

            return Ok(new { pdfPath = fullPdfUrl });
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao atualizar a planilha: {ex.Message}");
        }
    }

    [HttpGet("Relatorios")]
    public async Task<IActionResult> GetAllRelatoriosAsync()
    {
        try
        {
            if (HttpContext.Items["User"] is not FirebaseToken decodedToken)
            {
                return Unauthorized(new { Message = "User not authenticated." });
            }

            var relatorioRef = _firestoreDb.Collection("relatorios");

            var snapshot = await relatorioRef.GetSnapshotAsync();

            var relatorios = new List<Dictionary<string, object>>();

            foreach (var document in snapshot.Documents)
            {
                var data = document.ToDictionary();
                if (data.TryGetValue("UserId", out var userId) && userId?.ToString() == decodedToken.Uid)
                {
                    if (data.TryGetValue("data_fim", out var dataField))
                    {
                        string? dateTimeFimString = null;

                        if (dataField is Timestamp firestoreTimestamp)
                        {
                            // Converte Timestamp para DateTime
                            var dateTimeValue = firestoreTimestamp.ToDateTime();
                            dateTimeFimString = dateTimeValue.ToString("dd-MM-yyyy");
                        }

                        // Substitui o valor no dicionário se a conversão foi realizada
                        if (dateTimeFimString != null)
                        {
                            data["data_fim"] = dateTimeFimString;
                        }
                    }
                    if (data.TryGetValue("data_inicio", out var dataField2))
                    {
                        string? dateTimeInicioString = null;

                        if (dataField is Timestamp firestoreTimestamp)
                        {
                            // Converte Timestamp para DateTime
                            var dateTimeValue = firestoreTimestamp.ToDateTime();
                            dateTimeInicioString = dateTimeValue.ToString("dd-MM-yyyy");
                        }

                        // Substitui o valor no dicionário se a conversão foi realizada
                        if (dateTimeInicioString != null)
                        {
                            data["data_inicio"] = dateTimeInicioString;
                        }
                    }
                    if (data.TryGetValue("data_reg", out var dataField3))
                    {
                        string? dateTimeRegString = null;

                        if (dataField is Timestamp firestoreTimestamp)
                        {
                            // Converte Timestamp para DateTime
                            var dateTimeValue = firestoreTimestamp.ToDateTime();
                            dateTimeRegString = dateTimeValue.ToString("dd-MM-yyyy");
                        }

                        // Substitui o valor no dicionário se a conversão foi realizada
                        if (dateTimeRegString != null)
                        {
                            data["data_reg"] = dateTimeRegString;
                        }
                    }
                    relatorios.Add(data);
                }  
            }
            return Ok(relatorios);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao obter os relatórios: {ex.Message}");
            return BadRequest(ex.Message);
        }
    }
}