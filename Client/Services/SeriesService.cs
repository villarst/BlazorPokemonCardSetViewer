using System.Text.Json;
using BlazorPokemonCardSetViewer.Contracts;
using Shared.Models;

namespace BlazorPokemonCardSetViewer.Services;

public class SeriesService : ISeriesService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SeriesService> _logger;

    public SeriesService(HttpClient httpClient, ILogger<SeriesService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        
        // The 'BaseAddress' variable should be set in Program.cs
        _logger.LogInformation("CardService created with base address: {BaseAddress}", _httpClient.BaseAddress);
    }

    public async Task<List<PokemonSet>?> GetSeriesAsync(string seriesName)
    {
        try
        {
            _logger.LogInformation("Calling API for series based on name: {seriesName}", seriesName);
            
            // URL below is called as an example: 'https://api.pokemontcg.io/v2/sets?q=series:Scarlet & Violet'
            var response = await _httpClient.GetAsync($"api/PokemonCard/sets?q=series:{Uri.EscapeDataString(seriesName)}");
            _logger.LogInformation("API response status: {StatusCode}", response.StatusCode);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Response content: {StatusCode}", response.StatusCode);
                var sets = JsonSerializer.Deserialize<List<PokemonSet>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (sets != null)
                {
                    _logger.LogInformation("Found {Count} sets", sets.Count);
                    return sets;
                }
                _logger.LogWarning("Search result is null");
                return new List<PokemonSet>();
            }
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Error response: {ErrorContent}", errorContent);
            throw new Exception($"Error searching sets: {response.StatusCode}, {errorContent}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in CardService SearchSetsAsync");
            throw;
        }
    }
}