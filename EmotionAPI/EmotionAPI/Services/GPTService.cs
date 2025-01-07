using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System;

public class ChatGptService
{
    private readonly string _apiKey = "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";
    private readonly string _apiUrl = "https://api.openai.com/v1/completions";
    private readonly HttpClient _httpClient;

    public ChatGptService()
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + _apiKey); // Usando a chave diretamente aqui
    }


    public async Task<string> GerarRelatorio(string dados)
    {
        var requestBody = new
        {
            model = "gpt-3.5-turbo",  // Usando o modelo GPT-3.5-Turbo para maior eficiência
            prompt = $"Analise os seguintes dados e gere um relatório: {dados}",
            max_tokens = 1500, // Ajuste conforme necessário
            temperature = 0.7,
        };

        var jsonRequestBody = JsonConvert.SerializeObject(requestBody);
        var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(_apiUrl, content);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<OpenAiResponse>(responseContent);
            return result.Choices[0].Text.Trim();  // Retorna o texto gerado
        }
        else
        {
            throw new Exception($"Erro na API do ChatGPT: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}");
        }
    }

    // Classe para deserializar a resposta da API
    public class OpenAiResponse
    {
        public Choice[] Choices { get; set; }
    }

    public class Choice
    {
        public string Text { get; set; }
    }
}
