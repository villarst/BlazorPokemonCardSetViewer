using System.Text.Json;
using BlazorPokemonCardSetViewer.Contracts;
using Shared.Models;

namespace BlazorPokemonCardSetViewer.Services;

public class SetsService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SetsService> _logger;

    public SetsService(HttpClient httpClient, ILogger<SetsService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        
        // The 'BaseAddress' variable should be set in Program.cs
        _logger.LogInformation("CardService created with base address: {BaseAddress}", _httpClient.BaseAddress);
    }
}