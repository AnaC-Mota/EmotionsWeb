using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class VertexAiService
{
    private readonly string projectId = "emocoes-4f9b5";
    private readonly string location = "us-central1";
    private readonly string publisher = "google";
    private readonly string model = "gemini-2.0-flash-001";
    private readonly GoogleCredential credential;

    public VertexAiService()
    {
        string keyPath = Path.Combine(Directory.GetCurrentDirectory(), "vertexAI.json");
        credential = GoogleCredential.FromFile(keyPath)
            .CreateScoped("https://www.googleapis.com/auth/cloud-platform");
    }

    public async Task<string> GerarInsightsAsync(List<Dictionary<string, object>> registros)
    {
        string prompt = MontarPrompt(registros);
        var token = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
        string apiUrl = $"https://{location}-aiplatform.googleapis.com/v1/projects/{projectId}/locations/{location}/publishers/{publisher}/models/{model}:streamGenerateContent";

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    role = "user",
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            }
        };

        Console.WriteLine($"Chamando endpoint: {apiUrl}");
        Console.WriteLine($"Prompt enviado:\n{prompt}");
        Console.WriteLine($"Corpo da requisição:\n{JsonSerializer.Serialize(requestBody, new JsonSerializerOptions { WriteIndented = true })}");

        try
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

                using (var request = new HttpRequestMessage(HttpMethod.Post, apiUrl))
                {
                    request.Content = content;
                    using (var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            Console.WriteLine("Resposta bem-sucedida (streaming iniciado).");
                            return await ProcessarRespostaStream(response);
                        }
                        else
                        {
                            var errorContent = await response.Content.ReadAsStringAsync();
                            Console.WriteLine($"Erro API: {response.StatusCode}");
                            Console.WriteLine($"Detalhes: {errorContent}");
                            return $"Erro ao gerar insights. Código: {response.StatusCode}, Detalhes: {errorContent}";
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
            Console.WriteLine($"Stack: {ex.StackTrace}");
            return $"Erro ao chamar API: {ex.Message}";
        }
    }

    private async Task<string> ProcessarRespostaStream(HttpResponseMessage response)
    {
        using (var stream = await response.Content.ReadAsStreamAsync())
        using (var reader = new StreamReader(stream))
        {
            string fullResponse = "";
            string currentChunk = "";
            while (!reader.EndOfStream)
            {
                string line = await reader.ReadLineAsync();
                if (!string.IsNullOrWhiteSpace(line))
                {
                    currentChunk += line;
                    try
                    {
                        // Tenta analisar o chunk atual como um array de objetos JSON
                        JsonDocument[] docs = JsonSerializer.Deserialize<JsonDocument[]>(currentChunk);
                        if (docs != null)
                        {
                            foreach (var doc in docs)
                            {
                                if (doc.RootElement.TryGetProperty("candidates", out JsonElement candidates) &&
                                    candidates.ValueKind == JsonValueKind.Array &&
                                    candidates.GetArrayLength() > 0)
                                {
                                    JsonElement firstCandidate = candidates[0];
                                    if (firstCandidate.TryGetProperty("content", out JsonElement content) &&
                                        content.TryGetProperty("parts", out JsonElement parts) &&
                                        parts.ValueKind == JsonValueKind.Array &&
                                        parts.GetArrayLength() > 0)
                                    {
                                        if (parts[0].TryGetProperty("text", out JsonElement textElement))
                                        {
                                            fullResponse += textElement.GetString();
                                        }
                                    }
                                }
                            }
                            currentChunk = ""; // Resetar após processar um array de objetos
                        }
                    }
                    catch (JsonException)
                    {
                        // O chunk ainda não forma um array de objetos completo, continuar acumulando
                    }
                }
            }
            Console.WriteLine($"Resposta completa processada:\n{fullResponse}");
            return fullResponse;
        }
    }

    private string MontarPrompt(List<Dictionary<string, object>> registros)
    {
        string prompt = "Você é um psicólogo virtual. Analise os registros emocionais abaixo e gere um relatório com padrões, sugestões e conselhos:\n\n";
        foreach (var r in registros)
        {
            prompt += $"- Título: {r["titulo"]}, Emoção: {r["emocao"]}, Score: {r["score"]}, Magnitude: {r["magnitude"]}, Descrição: {r["descricao"]}\n";
        }
        return prompt + "\n\nEscreva de forma empática e informativa.";
    }
}