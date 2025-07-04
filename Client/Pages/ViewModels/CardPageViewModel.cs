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
    
    [Reactive] public PokemonCard? Card { get; private set; }
    [Reactive] public string CardId { get; set; } = "xy1-1"; // Default search parameter used.
    [Reactive] public bool IsLoading { get; private set; }
    [Reactive] public string? ErrorMessage { get; private set; }
    
    public CardPageViewModel(ILogger<CardPageViewModel> logger, ICardService cardService)
    {
        _logger = logger;
        _cardService = cardService;
        _logger.LogDebug("CardPageViewModel created.");
    }
    
    public async Task LoadCardAsync(string cardId)
    {
        IsLoading = true;
        ErrorMessage = null;
        try
        {
            _logger.LogInformation("Requesting card: {CardId}", cardId);
            var card = await _cardService.GetCardAsync(cardId);
            Card = card;
            
            if (card != null)
            {
                _logger.LogInformation("Card loaded: {CardName}", card.Name);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load card {cardId}.";
            _logger.LogError(ex, "Error loading card {CardId}", cardId);
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    public void Dispose()
    {
        _disposables?.Dispose();
        _logger.LogInformation("CardPageViewModel disposed");
    }
}