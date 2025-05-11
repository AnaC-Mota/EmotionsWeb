using Microsoft.AspNetCore.Mvc;
using NewsAPI;
using NewsAPI.Constants;
using NewsAPI.Models;
using System;
using System.Linq;

[Route("api/news")]
[ApiController]
public class NewsController : ControllerBase
{
    private readonly string apiKey = "c7920f7942a8441b9eb8c61d1ab961b8"; // Substitua pela sua chave válida

    [HttpGet]
    public IActionResult GetNews()
    {
        try
        {
            var newsApiClient = new NewsApiClient(apiKey);
            var articlesResponse = newsApiClient.GetEverything(new EverythingRequest
            {
                Q = "saúde mental OR bem-estar OR depressão OR ansiedade OR felicidade OR mindfulness OR psicologia OR autocuidado OR terapia",
                SortBy = SortBys.Relevancy,
                Language = Languages.PT,
                From = DateTime.UtcNow.AddDays(-7),
                To = DateTime.UtcNow
            });

            if (articlesResponse.Status == Statuses.Ok)
            {
                // Filtrar apenas artigos que possuem título e descrição válidos
                var filteredArticles = articlesResponse.Articles
                    .Where(article =>
                        !string.IsNullOrEmpty(article.Title) &&
                        (
                            article.Title.Contains("mental", StringComparison.OrdinalIgnoreCase) ||
                            article.Title.Contains("bem-estar", StringComparison.OrdinalIgnoreCase) ||
                            article.Title.Contains("depressão", StringComparison.OrdinalIgnoreCase) ||
                            article.Title.Contains("ansiedade", StringComparison.OrdinalIgnoreCase) ||
                            article.Title.Contains("felicidade", StringComparison.OrdinalIgnoreCase) ||
                            article.Title.Contains("mindfulness", StringComparison.OrdinalIgnoreCase) ||
                            article.Title.Contains("psicologia", StringComparison.OrdinalIgnoreCase) ||
                            article.Title.Contains("autocuidado", StringComparison.OrdinalIgnoreCase) ||
                            article.Title.Contains("terapia", StringComparison.OrdinalIgnoreCase)
                        ) ||
                        (!string.IsNullOrEmpty(article.Description) &&
                            (
                                article.Description.Contains("saúde", StringComparison.OrdinalIgnoreCase) ||
                                article.Description.Contains("mental", StringComparison.OrdinalIgnoreCase) ||
                                article.Description.Contains("bem-estar", StringComparison.OrdinalIgnoreCase) ||
                                article.Description.Contains("depressão", StringComparison.OrdinalIgnoreCase) ||
                                article.Description.Contains("ansiedade", StringComparison.OrdinalIgnoreCase) ||
                                article.Description.Contains("felicidade", StringComparison.OrdinalIgnoreCase) ||
                                article.Description.Contains("mindfulness", StringComparison.OrdinalIgnoreCase) ||
                                article.Description.Contains("psicologia", StringComparison.OrdinalIgnoreCase) ||
                                article.Description.Contains("autocuidado", StringComparison.OrdinalIgnoreCase) ||
                                article.Description.Contains("terapia", StringComparison.OrdinalIgnoreCase)
                            )
                        )
                    )
                    .ToList();

                return Ok(filteredArticles);
            }

            return BadRequest(new { message = "Erro ao buscar notícias." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno no servidor.", error = ex.Message });
        }
    }
}
