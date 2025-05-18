using System.Text.Json;
using BlazorPokemonCardSetViewer.Contracts;
using ReactiveUI;
using Shared.Models;

namespace BlazorPokemonCardSetViewer.Services;
public class CardService : ReactiveObject, ICardService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CardService> _logger;
    
    public CardService(HttpClient httpClient, ILogger<CardService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        // The BaseAddress should be set in Program.cs
        _logger.LogInformation("CardService created with base address: {BaseAddress}", _httpClient.BaseAddress);
    }
    
    public async Task<PokemonCard?> GetCardAsync(string cardId)
    {
        try
        {
            _logger.LogInformation("Calling API for card: {CardId}.", cardId);
            // THIS IS IMPORTANT - call the server API.
            var response = await _httpClient.GetAsync($"api/PokemonCard/{cardId}");
            
            _logger.LogInformation("API response status: {StatusCode}", response.StatusCode);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("Response content: {Content}", content);
                
                var card = JsonSerializer.Deserialize<PokemonCard>(content, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                if (card != null)
                {
                    _logger.LogInformation("Card deserialized: {CardName}", card.Name);
                    return card;
                }

                _logger.LogWarning("Deserialized card is null");
                throw new Exception("Failed to deserialize card data");
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Error response: {ErrorContent}", errorContent);
            throw new Exception($"Error fetching card: {response.StatusCode}, {errorContent}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in CardService");
            throw;
        }
    }
}