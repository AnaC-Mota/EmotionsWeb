using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

public class NewService
{
    private readonly HttpClient _httpClient;
    private const string apiUrl = "https://newsapi.org/v2/everything?q=bitcoin&apiKey=c7920f7942a8441b9eb8c61d1ab961b8";

    public NewService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Article>> GetMentalHealthNewsAsync()
    {
        var response = await _httpClient.GetAsync(apiUrl);
        if (!response.IsSuccessStatusCode)
        {
            return new List<Article>();
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var newsResult = JsonSerializer.Deserialize<NewsResponse>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return newsResult?.Articles ?? new List<Article>();
    }
}

public class NewsResponse
{
    public List<Article> Articles { get; set; }
}

public class Article
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Url { get; set; }
}
