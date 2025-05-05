using Shared;

namespace BlazorPokemonCardSetViewer.Contracts;

public interface IWeatherService
{
    Task<WeatherForecast[]> GetForecastAsync();
}