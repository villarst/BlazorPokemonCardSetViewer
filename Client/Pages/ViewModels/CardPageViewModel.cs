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
    private readonly ICardsService _cardService;
    private readonly CompositeDisposable _disposables = new CompositeDisposable();
    
    [Reactive] public PagedList<PokemonCard> PagedCards { get; private set; } = new PagedList<PokemonCard>();
    [Reactive] public string SearchTerm { get; set; } = "";
    [Reactive] public bool IsLoading { get; private set; }
    [Reactive] public string? ErrorMessage { get; private set; }
    [Reactive] public int CurrentPage { get; set; } = 1;
    [Reactive] public int PageSize { get; set; } = 12;
    
    public CardPageViewModel(ILogger<CardPageViewModel> logger, ICardsService cardService)
    {
        _logger = logger;
        _cardService = cardService;
        _logger.LogDebug("CardPageViewModel created.");
    }
    
    public async Task LoadCardsAsync(int? pageNumber = null)
    {
        if (string.IsNullOrWhiteSpace(SearchTerm))
        {
            ErrorMessage = "Please enter a search term";
            return;
        }
        
        IsLoading = true;
        ErrorMessage = null;
        
        if (pageNumber.HasValue)
            CurrentPage = pageNumber.Value;
        
        try
        {
            var request = new PagedRequest
            {
                SearchTerm = SearchTerm,
                PageNumber = CurrentPage,
                PageSize = PageSize
            };
            
            _logger.LogInformation("Requesting cards: {SearchTerm}, Page: {PageNumber}", 
                SearchTerm, CurrentPage);
                
            var result = await _cardService.GetCardsAsync(request);
            PagedCards = result;
            
            if (result.Data.Any())
            {
                _logger.LogInformation("Loaded {Count} cards of {Total} total", 
                    result.Data.Count, result.TotalCount);
            }
            else
            {
                ErrorMessage = $"No cards found for '{SearchTerm}'";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load cards for {SearchTerm}.";
            _logger.LogError(ex, "Error loading cards for {SearchTerm}", SearchTerm);
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    public async Task GoToPageAsync(int pageNumber)
    {
        if (pageNumber >= 1 && pageNumber <= PagedCards.TotalPages)
        {
            await LoadCardsAsync(pageNumber);
        }
    }
    
    public async Task NextPageAsync()
    {
        if (PagedCards.HasNextPage)
        {
            await GoToPageAsync(CurrentPage + 1);
        }
    }
    
    public async Task PreviousPageAsync()
    {
        if (PagedCards.HasPreviousPage)
        {
            await GoToPageAsync(CurrentPage - 1);
        }
    }
    
    public void Dispose()
    {
        _disposables?.Dispose();
        _logger.LogInformation("CardPageViewModel disposed");
    }
}