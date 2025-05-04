using BlazorPokemonCardSetViewer.Models;

namespace BlazorPokemonCardSetViewer.Contracts;

public interface IWeatherService
{
    Task<WeatherForecast[]> GetForecastAsync();
}