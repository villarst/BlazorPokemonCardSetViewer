using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Server.Features;
using Shared.Models;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PokemonCardController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PokemonCardController> _logger;
    private const string ApiKey = "15625e63-354d-4ce5-a221-a5c200ce57f4";
    
    public PokemonCardController(IHttpClientFactory httpClientFactory, ILogger<PokemonCardController> logger)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri("https://api.pokemontcg.io/v2/");
        _httpClient.DefaultRequestHeaders.Add("X-Api-Key", ApiKey);
    }
    
    [HttpGet("{cardId}")]
    public async Task<ActionResult<PokemonCard>> GetCard(string cardId)
    {
        try
        {
            _logger.LogInformation("Getting card: {CardId}", cardId);
            var response = await _httpClient.GetAsync($"cards/{cardId}");
            
            _logger.LogInformation("API Response: {StatusCode}", response.StatusCode);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("Response content: {Content}", content);
                
                var cardResponse = JsonSerializer.Deserialize<PokemonCardResponse>(content, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                if (cardResponse?.Data == null)
                {
                    _logger.LogWarning("Card data is null");
                    return NotFound("Card data not found");
                }
                
                // Map only the properties we need
                var card = new PokemonCard
                {
                    Id = cardResponse.Data.Id ?? string.Empty,
                    Name = cardResponse.Data.Name ?? string.Empty
                };
                
                return Ok(card);
            }
            
            return StatusCode((int)response.StatusCode, 
                $"Error fetching card: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting card {CardId}", cardId);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}