using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using BlazorPokemonCardSetViewer.Contracts;
using Shared.Models;
using System.Reactive.Linq;
using System.Reactive.Disposables;

namespace BlazorPokemonCardSetViewer.Pages.ViewModels;
public class CardPageViewModel : ReactiveObject, IDisposable
{
    private readonly ILogger<CardPageViewModel> _logger;
    private readonly ICardService _cardService;
    private readonly CompositeDisposable _disposables = new CompositeDisposable();
    
    [Reactive] public PokemonCard? Card { get; private set; } = null;
    [Reactive] public string CardId { get; set; } = "xy1-1"; // Default card ID
    [Reactive] public bool IsLoading { get; private set; }
    [Reactive] public string? ErrorMessage { get; private set; } = null;
    
    public ReactiveCommand<string, PokemonCard?> LoadCardCommand { get; }
    
    public CardPageViewModel(ILogger<CardPageViewModel> logger, ICardService cardService)
    {
        _logger = logger;
        _cardService = cardService;
        _logger.LogDebug("CardPageViewModel created.");
        
        LoadCardCommand = ReactiveCommand.CreateFromTask<string, PokemonCard?>(
            async (cardId) => {
                IsLoading = true;
                ErrorMessage = null;
                try
                {
                    _logger.LogInformation("Requesting card: {CardId}", cardId);
                    return await _cardService.GetCardAsync(cardId);
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Failed to load card: {ex.Message}";
                    _logger.LogError(ex, "Error loading card {CardId}", cardId);
                    return null;
                }
                finally
                {
                    IsLoading = false;
                }
            });
            
        LoadCardCommand.Subscribe(card => {
            Card = card;
            if (card != null)
            {
                _logger.LogInformation("Card loaded: {CardName}", card.Name);
            }
        }).DisposeWith(_disposables);
    }
    
    public void Dispose()
    {
        _disposables?.Dispose();
        _logger.LogInformation("CardPageViewModel disposed");
    }
}