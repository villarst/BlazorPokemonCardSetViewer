using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using BlazorPokemonCardSetViewer.Contracts;
using Shared.Models;
using System.Reactive.Disposables;

namespace BlazorPokemonCardSetViewer.Pages.ViewModels;

public class SeriesPageViewModel : ReactiveObject, IDisposable
{
    private readonly ILogger<SeriesPageViewModel> _logger;
    private readonly ISeriesService _seriesService;
    private readonly CompositeDisposable _disposables = new CompositeDisposable();
    
    [Reactive] public PokemonSet? Sets { get; set; }
    [Reactive] public string Series { get; set; } = "Scarlet & Violet"; // Default set name used
    [Reactive] public bool IsLoading { get; set; }
    [Reactive] public string? ErrorMessage { get; set; }

    public SeriesPageViewModel(ILogger<SeriesPageViewModel> logger, ISeriesService seriesService)
    {
        _logger = logger;
        _seriesService = seriesService;
        _logger.LogInformation("SetsPageViewModel created");
    }

    public async Task LoadSetAsync(string setName)
    {
        IsLoading = true;
        ErrorMessage = null;
        try
        {
            _logger.LogInformation("Requesting sets: {SetName}", setName);
            var sets = await _seriesService.GetSeriesAsync(setName);
            Sets = sets;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public void Dispose()
    {
        _disposables?.Dispose();
        _logger.LogInformation("CardPageViewModel disposed");
    }
}