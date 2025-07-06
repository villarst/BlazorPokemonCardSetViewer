using System.Text.Json;
using BlazorPokemonCardSetViewer.Contracts;
using ReactiveUI;
using Shared.Models;

namespace BlazorPokemonCardSetViewer.Services;

public class CardsService : ReactiveObject, ICardsService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CardsService> _logger;
    
    public CardsService(HttpClient httpClient, ILogger<CardsService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _logger.LogInformation("CardService created with base address: {BaseAddress}", httpClient.BaseAddress);
    }
    
    public async Task<PagedList<PokemonCard>> GetCardsAsync(PagedRequest request)
    {
        try
        {
            _logger.LogInformation("Calling API for cards: {SearchTerm}, Page: {PageNumber}", 
                request.SearchTerm, request.PageNumber);
                
            var response = await _httpClient.GetAsync(
                $"api/PokemonCard/{request.SearchTerm}?pageNumber={request.PageNumber}&pageSize={request.PageSize}");
            
            _logger.LogInformation("API response status: {StatusCode}", response.StatusCode);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("Response content: {Content}", content);
                
                var result = JsonSerializer.Deserialize<PagedList<PokemonCard>>(content, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                if (result != null)
                {
                    _logger.LogInformation("Deserialized {Count} cards of {Total} total", 
                        result.Data.Count, result.TotalCount);
                    return result;
                }
                
                _logger.LogWarning("Deserialized result is null");
                return new PagedList<PokemonCard>();
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Error response: {ErrorContent}", errorContent);
            throw new Exception($"Error fetching cards: {response.StatusCode}, {errorContent}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in CardService.GetCardsAsync");
            throw;
        }
    }
    
    public async Task<IEnumerable<PokemonCard>> GetAllCardsAsync(string searchTerm)
    {
        // Implementation for getting all cards without pagination if needed
        var request = new PagedRequest { SearchTerm = searchTerm, PageSize = 1000 };
        var result = await GetCardsAsync(request);
        return result.Data;
    }
}

// public class CardsService : ReactiveObject, ICardsService
// {
//     private readonly HttpClient _httpClient;
//     private readonly ILogger<CardsService> _logger;
//     
//     public CardsService(HttpClient httpClient, ILogger<CardsService> logger)
//     {
//         _httpClient = httpClient;
//         _logger = logger;
//         _logger.LogInformation("CardService created with base address: {BaseAddress}", httpClient.BaseAddress);
//     }
//     
//     // Implementation for multiple cards (main method)
//     public async Task<IEnumerable<PokemonCard>> GetCardsAsync(string searchTerm)
//     {
//         try
//         {
//             _logger.LogInformation("Calling API for cards with search term: {SearchTerm}.", searchTerm);
//             // Call the server API for multiple cards
//             var response = await _httpClient.GetAsync($"api/PokemonCard/{searchTerm}");
//             
//             _logger.LogInformation("API response status: {StatusCode}", response.StatusCode);
//             
//             if (response.IsSuccessStatusCode)
//             {
//                 var content = await response.Content.ReadAsStringAsync();
//                 _logger.LogDebug("Response content: {Content}", content);
//                 
//                 // deserialize to a list of PokemonCard objects
//                 var cards = JsonSerializer.Deserialize<List<PokemonCard>>(content, 
//                     new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
//                 
//                 if (cards != null)
//                 {
//                     _logger.LogInformation("Deserialized {Count} cards", cards.Count);
//                     return cards;
//                 }
//                 
//                 _logger.LogWarning("Deserialized cards list is null");
//                 return new List<PokemonCard>(); // Return empty list instead of throwing
//             }
//             
//             var errorContent = await response.Content.ReadAsStringAsync();
//             _logger.LogWarning("Error response: {ErrorContent}", errorContent);
//             throw new Exception($"Error fetching cards: {response.StatusCode}, {errorContent}");
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "Exception in CardService.GetCardsAsync");
//             throw;
//         }
//     }
//     
//     // keep the single card method optional)
//     public async Task<PokemonCard?> GetCardAsync(string cardName)
//     {
//         try
//         {
//             _logger.LogInformation("Calling API for single card: {CardName}.", cardName);
//             
//             // You could either:
//             // Option 1: Call the multiple cards endpoint and return the first result
//             var cards = await GetCardsAsync(cardName);
//             return cards.FirstOrDefault();
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "Exception in CardService.GetCardAsync");
//             throw;
//         }
//     }
// }