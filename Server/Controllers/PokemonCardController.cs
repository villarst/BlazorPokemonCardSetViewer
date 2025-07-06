using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Server.Features;
using Shared.Models;

namespace Server.Controllers;

// Having "Controller" in the file name lets ASP.NET know that it needs to be seperated into "PokemonCard" and
// "Controller". So [Route("api/[controller]")] becomes api/PokemonCard for your PokemonCardController class.
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
    
    [HttpGet("{searchTerm}")]
    public async Task<ActionResult<PagedList<PokemonCard>>> GetCards(
        string searchTerm, 
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 12)
    {
        try
        {
            _logger.LogInformation("Getting cards: {SearchTerm}, Page: {PageNumber}, Size: {PageSize}", 
                searchTerm, pageNumber, pageSize);
                
            var response = await _httpClient.GetAsync($"cards?q=name:{searchTerm}");
            _logger.LogInformation("API Response: {StatusCode}", response.StatusCode);
            
            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode,
                    $"Error fetching cards: {response.StatusCode}");
                    
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogDebug("Response content: {Content}", content);
                
            var cardsResponse = JsonSerializer.Deserialize<GetPokemonCardsResponse>(content, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
            if (cardsResponse?.Data == null || cardsResponse.Data.Length == 0)
            {
                _logger.LogWarning("No card data found");
                return Ok(new PagedList<PokemonCard>
                {
                    Data = new List<PokemonCard>(),
                    TotalCount = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                });
            }
                
            // Map all cards first
            var allCards = cardsResponse.Data.Select(cardData => new PokemonCard
            {
                Id = cardData.Id,
                Name = cardData.Name,
                Hp = cardData.Hp ?? "N/A",
                Images = new CardImages
                {
                    Small = cardData.Images?.Small,
                    Large = cardData.Images?.Large,
                }
            }).ToList();
            
            // Apply pagination
            var totalCount = allCards.Count;
            var pagedCards = allCards
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            
            var result = new PagedList<PokemonCard>
            {
                Data = pagedCards,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cards {SearchTerm}", searchTerm);
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    
    // // The requests here must have the "{cardName}" at the end of the URI.
    // // For instance, "https://localhost:5205/api/PokemonCard/xy1-1" works.
    // [HttpGet("{cardName}")]
    // public async Task<ActionResult<IEnumerable<PokemonCard>>> GetCard(string cardName)
    // {
    //     try
    //     {
    //         _logger.LogInformation("Getting card: {CardName}", cardName);
    //         var response = await _httpClient.GetAsync($"cards?q=name:{cardName}");
    //         _logger.LogInformation("API Response: {StatusCode}", response.StatusCode);
    //     
    //         if (!response.IsSuccessStatusCode)
    //             return StatusCode((int)response.StatusCode,
    //                 $"Error fetching card: {response.StatusCode}");
    //             
    //         var content = await response.Content.ReadAsStringAsync();
    //         _logger.LogDebug("Response content: {Content}", content);
    //         
    //         var cardsResponse = JsonSerializer.Deserialize<GetPokemonCardsResponse>(content, 
    //             new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    //         
    //         if (cardsResponse?.Data == null || cardsResponse.Data.Length == 0)
    //         {
    //             _logger.LogWarning("No card data found");
    //             return NotFound("No cards found");
    //         }
    //         
    //         // Map all cards
    //         var cards = cardsResponse.Data.Select(cardData => new PokemonCard
    //         {
    //             Id = cardData.Id,
    //             Name = cardData.Name,
    //             Hp = cardData.Hp ?? "N/A", // or cardData.Hp ?? string.Empty
    //             Images = new CardImages
    //             {
    //                 Small = cardData.Images?.Small,
    //                 Large = cardData.Images?.Large,
    //             }
    //         }).ToList();
    //     
    //         return Ok(cards);
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Error getting card {CardName}", cardName);
    //         return StatusCode(500, $"Internal server error: {ex.Message}");
    //     }
    // }
}