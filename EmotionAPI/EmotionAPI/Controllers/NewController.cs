using Microsoft.AspNetCore.Mvc;
using NewsAPI;
using NewsAPI.Constants;
using NewsAPI.Models;
using System.Threading.Tasks;

[Route("api/news")]
[ApiController]
public class NewsController : ControllerBase
{
    private readonly string apiKey = "c7920f7942a8441b9eb8c61d1ab961b8"; // Sua chave da NewsAPI

    [HttpGet]
    public IActionResult GetNews()
    {
        try
        {
            var newsApiClient = new NewsApiClient(apiKey);
            var articlesResponse = newsApiClient.GetEverything(new EverythingRequest
            {
                Q = "mental health", // Altere para seu tema
                SortBy = SortBys.Popularity,
                Language = Languages.PT,
                From = DateTime.UtcNow.AddDays(-7) // Últimos 7 dias
            });

            if (articlesResponse.Status == Statuses.Ok)
            {
                return Ok(articlesResponse.Articles); // Retorna os artigos para o frontend
            }

            return BadRequest(new { message = "Erro ao buscar notícias." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno no servidor.", error = ex.Message });
        }
    }
}