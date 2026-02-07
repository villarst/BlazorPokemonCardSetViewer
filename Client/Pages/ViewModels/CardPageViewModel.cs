using Microsoft.JSInterop;
using Shared.Models;
using System.Reactive.Disposables;
using BlazorPokemonCardSetViewer.Features.PokemonCard;
using BlazorPokemonCardSetViewer.Services;

namespace BlazorPokemonCardSetViewer.Pages.ViewModels;

public interface ICardPageViewModel
{
    PagedList<PokemonCardDataResponse> PagedCards { get; set; }
    PagedList<RarityResponse> Rarities { get; set; }
    Dictionary<string, bool> RarityNameAndValues { get; set; }
    string SearchTerm { get; set; }
    string CardId { get; set; }
    bool IsLoading { get; set; }
    string? ErrorMessage { get; set; }
    int CurrentPage { get; set; }
    int PageSize { get; set; }
    bool CheckIfRaritiesSelected();
}

public class CardPageViewModel (IJSRuntime js) : ICardPageViewModel, IDisposable
{
    private readonly IJSRuntime _js;
    private readonly ILogger<CardPageViewModel> _logger;
    private readonly ICardsService _cardService;
    private readonly CompositeDisposable _disposables = new();
    
    public PagedList<PokemonCardDataResponse> PagedCards { get; set; }
    public PagedList<RarityResponse> Rarities { get; set; }
    public Dictionary<string, bool>? RarityNameAndValues { get; set; } = new();
    public string SearchTerm { get; set; } = string.Empty;
    public string CardId { get; set; } = string.Empty;
    public bool IsLoading { get; set; }
    public string? ErrorMessage { get; set; }
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 12;
    
    public CardPageViewModel(ILogger<CardPageViewModel> logger, IJSRuntime js, ICardsService cardService) : this(js)
    {
        _logger = logger;
        _js = js;
        _cardService = cardService;
        _logger.LogDebug("CardPageViewModel created.");
        PagedCards =  new PagedList<PokemonCardDataResponse>();
        Rarities = new PagedList<RarityResponse>();
    }
    
    public void Dispose()
    {
        _disposables?.Dispose();
        _logger.LogInformation("CardPageViewModel disposed");
    }
    
    public async Task LoadCardsBySearchTermAsync(int? pageNumber = null)
    {
        List<string> rarityNamesAndValuesToSend = [];
        var rarityNamesAndValuesList = RarityNameAndValues!.ToList();
        rarityNamesAndValuesToSend
            .AddRange(
                from rarityNameAndValues in rarityNamesAndValuesList 
                where rarityNameAndValues.Value 
                select rarityNameAndValues.Key);

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
                PageSize = PageSize,
                // Need to CHANGE
                Rarities = rarityNamesAndValuesToSend,
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
    
    public async Task LoadCardByCardIdAsync(int? pageNumber = null)
    {
        if (string.IsNullOrWhiteSpace(CardId))
        {
            ErrorMessage = "Card Id is required.";
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
                CardId = CardId,
                PageNumber = CurrentPage,
                PageSize = PageSize
            };
            
            _logger.LogInformation("Requesting card with Card ID: {CardId}, Page: {PageNumber}", 
                CardId, CurrentPage);
                
            var result = await _cardService.GetCardByIdAsync(request);
            PagedCards = result;
            
            if (result.Data.Any())
            {
                _logger.LogInformation("Loaded {Count} card of {Total} total", 
                    result.Data.Count, result.TotalCount);
            }
            else
            {
                ErrorMessage = $"No card found with Card ID '{CardId}'";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load card with Card ID: {CardId}.";
            _logger.LogError(ex, "Error loading card for Card ID: {CardId}", CardId);
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task LoadRaritiesOfCards()
    {
        IsLoading = true;
        ErrorMessage = null;
        try
        {
            var request = new PagedRequest
            {
                PageNumber = 1,
                PageSize = 50
            };
            
            _logger.LogInformation("Requesting all cards.");
                
            var result = await _cardService.GetRaritiesAsync(request);
            Rarities = result;
            
            if (result.Data.Any())
            {
                _logger.LogInformation("Loaded {Count} rarities", 
                    result.Data.Count);
            }
            else
            {
                ErrorMessage = "No rarities found.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Failed to load rarities.";
            _logger.LogError(ex, "Failed to load rarities.");
        }
        finally
        {
            IsLoading = false;
            foreach (var rarity in Rarities.Data)
            {
                if (RarityNameAndValues!.ContainsKey(rarity.Name!)) continue;
                RarityNameAndValues.Add(rarity.Name!, false);
            }
        }
    }
    
    public async Task GoToPageAsync(int pageNumber)
    {
        if (pageNumber >= 1 && pageNumber <= PagedCards.TotalPages)
        {
            await LoadCardsBySearchTermAsync(pageNumber);
        }
    }
    
    public async Task NextPageAsync()
    {
        if (PagedCards.HasNextPage)
        {
            await GoToPageAsync(CurrentPage + 1);
            await CallJsScrollUp();
        }
    }
    
    public async Task PreviousPageAsync()
    {
        if (PagedCards.HasPreviousPage)
        {
            await GoToPageAsync(CurrentPage - 1);
        }
    }
    
    private async Task CallJsScrollUp()
    {
        try
        {
            await _js.InvokeVoidAsync("scrollToTop");
            
        }
        catch (Exception ex)
        {
            _logger.LogError("Error: {ExMessage}", ex.Message);
        }
    }

    public bool CheckIfRaritiesSelected()
    {
        return RarityNameAndValues!.ContainsValue(true);
    }
}