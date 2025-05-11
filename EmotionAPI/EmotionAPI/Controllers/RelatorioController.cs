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


public class ResultScores()
{
    public DateTime data { get; set; }
    public double scores { get; set; }
}
public class RelatorioController : Controller
{
    private readonly FirestoreDb _firestoreDb;
    private readonly PdfService _pdfService;
    private readonly GraficoService _graficoService;
    private readonly VertexAiService _vertexAiService;


    public RelatorioController(FirestoreDb firestoreDb,
                                PdfService pdfService,
                                GraficoService graficoService,
                                VertexAiService vertexAiService)
    {
        _firestoreDb = firestoreDb;
        _pdfService = pdfService;
        _graficoService = graficoService;
        _vertexAiService = vertexAiService;
    }

    [HttpPost("Grafico")]
    public async Task<IActionResult> CriarRelatorio([FromBody] FilterDTO filter)
    {
        if (HttpContext.Items["User"] is not FirebaseToken decodedToken)
            return Unauthorized();

        var entriesRef = _firestoreDb.Collection("entries");
        var snapshot = await entriesRef.GetSnapshotAsync();

        var documentos = new List<Dictionary<string, object>>();
        var datar = new List<ResultScores>();
        var contagemEmocoes = new Dictionary<string, int>();

        foreach (var doc in snapshot.Documents)
        {
            var data = doc.ToDictionary();
            if (data.TryGetValue("userId", out var uid) && uid?.ToString() == decodedToken.Uid)
            {
                if (data.TryGetValue("data", out var dataField) && dataField is Timestamp ts)
                {
                    DateTime dataRegistro = ts.ToDateTime();
                    if (filter.startDate.HasValue && dataRegistro < filter.startDate.Value) continue;
                    if (filter.endDate.HasValue && dataRegistro > filter.endDate.Value) continue;

                    documentos.Add(data);

                    if (data.TryGetValue("score", out var score) && double.TryParse(score.ToString(), out double scoreVal))
                        datar.Add(new ResultScores() { data = dataRegistro, scores = scoreVal });

                    if (data.TryGetValue("emocao", out var emocao))
                    {
                        string val = emocao.ToString();
                        if (contagemEmocoes.ContainsKey(val)) contagemEmocoes[val]++;
                        else contagemEmocoes[val] = 1;
                    }
                }
            }
        }

        // Gera insights com Vertex AI
        var insights = await _vertexAiService.GerarInsightsAsync(documentos);

        // Gráfico de emoções e score

        string imagemScore = _graficoService.GerarGraficoDeScore(datar.OrderBy(t => t.data).ToList());

        // Gerar PDF com insights e imagem
        var pdfBytes = _pdfService.GerarPdfComTextoEImagem(insights, imagemScore);
        string pdfFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files");
        Directory.CreateDirectory(pdfFolder);
        string pdfFileName = $"relatorio_{Guid.NewGuid()}.pdf";
        string pdfPath = Path.Combine(pdfFolder, pdfFileName);
        await System.IO.File.WriteAllBytesAsync(pdfPath, pdfBytes);

        string fullPdfUrl = $"{Request.Scheme}://{Request.Host}/files/{pdfFileName}";

        // Salvar no Firestore
        var relatorioRef = _firestoreDb.Collection("relatorios").Document();
        await relatorioRef.SetAsync(new Dictionary<string, object>
    {
        { "UserId", decodedToken.Uid },
        { "data_fim", Timestamp.FromDateTime(filter.endDate ?? DateTime.UtcNow) },
        { "data_inicio", Timestamp.FromDateTime(filter.startDate ?? DateTime.MinValue) },
        { "data_reg", Timestamp.FromDateTime(DateTime.UtcNow) },
        { "nome", filter.Title ?? "Relatório" },
        { "relatorio", fullPdfUrl }
    });

        return Ok(new { pdfUrl = fullPdfUrl });
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