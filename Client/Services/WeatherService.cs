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
        return await _httpClient.GetFromJsonAsync<WeatherForecast[]>("WeatherForecast");
    }
}
