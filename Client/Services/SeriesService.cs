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

    public async Task<PokemonSet?> GetSeriesAsync(string seriesName)
    {
        
    }
}