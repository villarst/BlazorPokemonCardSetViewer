using System.Text;
using System.Text.Json;
using BlazorPokemonCardSetViewer.Features.PokemonCard;
using Newtonsoft.Json;
using Shared.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace BlazorPokemonCardSetViewer.Services;

public interface ICardService
{
    Task<PagedList<PokemonCardDataResponse>> GetCardsAsync(PagedRequest request);
    Task<PagedList<PokemonCardDataResponse>> GetCardByIdAsync(PagedRequest request);
    Task<PagedList<RarityResponse>> GetRaritiesAsync(PagedRequest request);
}

public class CardService : ICardService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CardService> _logger;
    
    public CardService(HttpClient httpClient, ILogger<CardService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _logger.LogInformation("CardService created with base address: {BaseAddress}", httpClient.BaseAddress);
    }
    
    public async Task<PagedList<PokemonCardDataResponse>> GetCardsAsync(PagedRequest request)
    {
        try
        {
            _logger.LogInformation("Calling API for cards: {SearchTerm}, Page: {PageNumber}", 
                request.SearchTerm, request.PageNumber);

            _logger.LogInformation("Serializing the request for sending to the controller. ");
            var serializedObject = JsonConvert.SerializeObject(request);
            var parcelPackage = new StringContent(serializedObject, Encoding.UTF8, "application/json");
            _logger.LogInformation("Sending the request in a package with UTF8 encoding to api/PokemonCard/search");
            var response = await _httpClient.PostAsync("api/PokemonCard/search", parcelPackage);
            
            _logger.LogInformation("API response status: {StatusCode}", response.StatusCode);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("Response content: {Content}", content);
                
                var result = JsonSerializer.Deserialize<PagedList<PokemonCardDataResponse>>(content, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                if (result != null)
                {
                    _logger.LogInformation("Deserialized {Count} cards of {Total} total", 
                        result.Data.Count, result.TotalCount);
                    return result;
                }
                
                _logger.LogWarning("Deserialized result is null");
                return new PagedList<PokemonCardDataResponse>();
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Error response: {ErrorContent}", errorContent);
            throw new Exception($"Error fetching cards: {response.StatusCode}, {errorContent}");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Exception in CardService.GetCardsAsync");
            throw;
        }
    }

    public async Task<PagedList<PokemonCardDataResponse>> GetCardByIdAsync(PagedRequest request)
    {
        try
        {
            _logger.LogInformation("Calling API for card with ID: {CardId}, Page: {PageNumber}", 
                request.CardId, request.PageNumber);
                
            var response = await _httpClient.GetAsync(
                $"api/PokemonCard/id/{request.CardId}?pageNumber={request.PageNumber}&pageSize={request.PageSize}");
            
            _logger.LogInformation("API response status: {StatusCode}", response.StatusCode);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("Response content: {Content}", content);
                
                var result = JsonSerializer.Deserialize<PagedList<PokemonCardDataResponse>>(content, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                if (result != null)
                {
                    _logger.LogInformation("Deserialized {Count} card of {Total} total", 
                        result.Data.Count, result.TotalCount);
                    return result;
                }
                
                _logger.LogWarning("Deserialized result is null");
                return new PagedList<PokemonCardDataResponse>();
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Error response: {ErrorContent}", errorContent);
            throw new Exception($"Error fetching cards: {response.StatusCode}, {errorContent}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in CardService.GetCardsAsync");
            throw;
        }
    }
    
    public async Task<PagedList<RarityResponse>> GetRaritiesAsync(PagedRequest request)
    {
        try
        {
            _logger.LogInformation("Calling API rarities");
                
            var response = await _httpClient.GetAsync(
                $"api/PokemonCard/rarities");
            
            _logger.LogInformation("API response status: {StatusCode}", response.StatusCode);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("Response content: {Content}", content);
                
                var result = JsonSerializer.Deserialize<PagedList<RarityResponse>>(content, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                if (result != null)
                {
                    _logger.LogInformation("Deserialized {Count} rarities", 
                        result.Data.Count);
                    return result;
                }
                
                _logger.LogWarning("Deserialized result is null");
                return new PagedList<RarityResponse>();
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Error response: {ErrorContent}", errorContent);
            throw new Exception($"Error fetching rarities: {response.StatusCode}, {errorContent}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in CardService.GetRaritiesAsync");
            // TODO: Need to have a fallback instead of throwing.
            throw;
        }
    }
}