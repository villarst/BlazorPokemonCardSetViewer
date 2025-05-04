using BlazorPokemonCardSetViewer.Contracts;
using BlazorPokemonCardSetViewer.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace BlazorPokemonCardSetViewer.Pages.ViewModels;

public class WeatherViewModel : ReactiveObject, IDisposable
{
    private readonly IWeatherService _weatherService;
    [Reactive] private WeatherForecast[]? _forecasts { get; set; }
    [Reactive] private bool _isLoading { get; set; }
    private readonly ILogger<WeatherViewModel> _logger;

    public WeatherViewModel(IWeatherService weatherService, ILogger<WeatherViewModel> logger)
    {
        _logger = logger;
        _weatherService = weatherService;
        _logger.LogDebug("WeatherViewModel created.");
    }

    public async Task LoadForecastsAsync()
    {
        try
        {
            _isLoading = true;
            _forecasts = await _weatherService.GetForecastAsync();
        }
        finally
        {
            _isLoading = false;
        }
    }

    protected async Task OnInitializedAsync()
    {
        await LoadForecastsAsync();
    }

    public void Dispose()
    {
        _forecasts = null;
    }
}