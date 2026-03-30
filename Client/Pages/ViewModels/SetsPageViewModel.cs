using Microsoft.JSInterop;
using Shared.Models;
using BlazorPokemonCardSetViewer.Features.PokemonSet;
using BlazorPokemonCardSetViewer.Services;

namespace BlazorPokemonCardSetViewer.Pages.ViewModels;

public interface ISetsPageViewModel
{
    PagedList<PokemonSetDataResponse>? PagedSets { get; set; }

    Task LoadSets(int? pageNumber = null);

    string SortOrder { get; set; }
    bool IsLoading { get; set; }
    int CurrentPage { get; set; }
    int PageSize { get; set; }
}

public class SetsPageViewModel : ISetsPageViewModel, IDisposable
{
    private readonly IJSRuntime _js;
    private readonly ILogger<SetsPageViewModel> _logger;
    private readonly ISetsService _setsService;
    
    public PagedList<PokemonSetDataResponse>? PagedSets { get; set; }

    public string SortOrder { get; set; } = "newest";
    
    public bool IsLoading { get; set; }
    
    public string? ErrorMessage { get; set; }
    
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 12;

    
    public SetsPageViewModel(ILogger<SetsPageViewModel> logger, IJSRuntime js, ISetsService setsService)
    {
        _logger = logger;
        _js = js;
        _setsService = setsService;
    }

    public async Task LoadSets(int? pageNumber = null)
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;
            
            if (pageNumber.HasValue)
                CurrentPage = pageNumber.Value;

            var request = new PagedRequest
            {
                PageNumber = CurrentPage,
                PageSize = PageSize,
                SortOrder = "newest",
            };
            
            _logger.LogInformation("Requesting sets: {SortOrder}, Page: {PageNumber}", 
                SortOrder, CurrentPage);
            
            var result = await _setsService.GetSetsAsync(request);
            PagedSets = result;

            if (result.Data.Any())
            {
                _logger.LogInformation("Loaded {Count} sets of {Total} total", 
                    result.Data.Count, result.TotalCount);
            }
            else
            {
                ErrorMessage = "No sets found";
            }
            
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load sets.";
            _logger.LogError(ex, "Error loading sets");
        }
        finally
        {
            IsLoading = false;
        }        
    }
        
    public void Dispose()
    {
        PagedSets = null;
        _logger.LogInformation("SetsPageViewModel disposed");
    }
}