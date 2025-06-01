using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Server.Features;
using Shared.Models;

namespace Server.Controllers;

// Having "Controller" in the file name lets ASP.NET know that it needs to be seperated into "PokemonCard" and
// "Controller". So [Route("api/[controller]")] becomes api/PokemonCard for your PokemonCardController class.
[ApiController]
[Route("api/[controller]")]
public class PokemonSeriesController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PokemonSeriesController> _logger;
    private const string ApiKey = "15625e63-354d-4ce5-a221-a5c200ce57f4";

    public PokemonSeriesController(IHttpClientFactory httpClientFactory, ILogger<PokemonSeriesController> logger)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri("https://api.pokemontcg.io/v2/");
        _httpClient.DefaultRequestHeaders.Add("X-Api-Key", ApiKey);
    }
    
    // We will call the URL example below and it must have {seriesName} 
    // URL below is called as an example: 'https://api.pokemontcg.io/v2/sets?q=series:{seriesName}'
    [HttpGet("{seriesName}")]
    public async Task<ActionResult<PokemonSeries>> GetSeries(string seriesName)
    {
        try
        {
            _logger.LogInformation("Getting series with: {SeriesName}", seriesName);
            var response = await _httpClient.GetAsync($"sets?q=series:{seriesName}");
            
            _logger.LogInformation("API Response: {StatusCode}", response.StatusCode);
            
            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode,
                    $"Error fetching card: {response.StatusCode}");
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogDebug("Response content: {Content}", content);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting series {SeriesName}", seriesName);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}