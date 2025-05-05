using System.Net.Http.Json;
using BlazorPokemonCardSetViewer.Contracts;
using BlazorPokemonCardSetViewer.Models;
using ReactiveUI;

namespace BlazorPokemonCardSetViewer.Services;

public class WeatherService : ReactiveObject, IWeatherService
{
    private readonly HttpClient _httpClient;
    
    public WeatherService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<WeatherForecast[]> GetForecastAsync()
    {
        // Make sure the URL is correct and returns valid JSON
        return await _httpClient.GetFromJsonAsync<WeatherForecast[]>("WeatherForecast") ?? Array.Empty<WeatherForecast>();
    }
}
