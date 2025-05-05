using System.Net.Http.Json;
using BlazorPokemonCardSetViewer.Contracts;
using Shared;

using ReactiveUI;

namespace BlazorPokemonCardSetViewer.Services;

public class WeatherService : ReactiveObject, IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WeatherService> _logger;
    
    public WeatherService(HttpClient httpClient, ILogger<WeatherService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<WeatherForecast[]> GetForecastAsync()
    {
        try
        {
            // Make sure this URL is correct - should point to your server's port
            return await _httpClient.GetFromJsonAsync<WeatherForecast[]>("weatherforecast") ?? 
                   Array.Empty<WeatherForecast>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching weather data");
            return Array.Empty<WeatherForecast>();
        }
    }
}
