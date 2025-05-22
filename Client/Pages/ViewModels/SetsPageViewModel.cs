using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using BlazorPokemonCardSetViewer.Contracts;
using Shared.Models;
using System.Reactive.Disposables;

namespace BlazorPokemonCardSetViewer.Pages.ViewModels;

public class SetsPageViewModel : ReactiveObject, IDisposable
{
    private readonly ILogger<SetsPageViewModel> _logger;
    private readonly ICardService _cardService;
    private readonly CompositeDisposable _disposables = new CompositeDisposable();

    public SetsPageViewModel(ILogger<SetsPageViewModel> logger, ICardService cardService)
    {
        _logger = logger;
        _cardService = cardService;
        _logger.LogInformation("SetsPageViewModel created");
    }
    
    public void Dispose()
    {
        _disposables?.Dispose();
        _logger.LogInformation("CardPageViewModel disposed");
    }
}