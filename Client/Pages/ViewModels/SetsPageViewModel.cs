using Microsoft.JSInterop;
using Shared.Models;
using BlazorPokemonCardSetViewer.Features.PokemonSet;
using BlazorPokemonCardSetViewer.Services;

namespace BlazorPokemonCardSetViewer.Pages.ViewModels;

public interface ISetsPageViewModel
{
    PagedList<PokemonSetDataResponse>? PagedSets { get; set; }
}

public class SetsPageViewModel : ISetsPageViewModel, IDisposable
{
    private readonly IJSRuntime _js;
    private readonly ILogger<SetsPageViewModel> _logger;
    private readonly ISetsService _setsService;
    
    public PagedList<PokemonSetDataResponse>? PagedSets { get; set; }

    public SetsPageViewModel(ILogger<SetsPageViewModel> logger, IJSRuntime js, ISetsService setsService)
    {
        _logger = logger;
        _js = js;
        _setsService = setsService;
    }
        
    public void Dispose()
    {
        PagedSets = null;
        _logger.LogInformation("SetsPageViewModel disposed");
    }
}