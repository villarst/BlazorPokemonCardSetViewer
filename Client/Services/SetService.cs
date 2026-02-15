using System.Text;
using System.Text.Json;
using BlazorPokemonCardSetViewer.Features.PokemonSet;
using Newtonsoft.Json;
using Shared.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace BlazorPokemonCardSetViewer.Services;

public interface ISetService
{
    Task<PagedList<PokemonSetDataResponse>> GetSetsAsync(PagedRequest request);
}

public class SetService : ISetService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SetService> _logger;

    public SetService(HttpClient httpClient, ILogger<SetService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _logger.LogInformation("SetService created with base address: {BaseAddress}", httpClient.BaseAddress);
    }

    public async Task<PagedList<PokemonSetDataResponse>> GetSetsAsync(PagedRequest request)
    {
        try
        {
            _logger.LogInformation("Calling API for sets, Page; {PageNumber}", request.PageNumber);
            var response = await _httpClient.GetAsync(
                $"api/PokemonSet/sets?pageNumber={request.PageNumber}&pageSize{request.PageSize}");

            _logger.LogInformation("API response status: {StatusCode}", response.StatusCode);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("Response content: {Content}", content);

                var result = JsonSerializer.Deserialize<PagedList<PokemonSetDataResponse>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (result != null)
                {
                    _logger.LogInformation("Deserialized {Count} sets",
                        result.Data.Count);
                    return result;
                }

                _logger.LogWarning("Deserialized result is null");
                return new PagedList<PokemonSetDataResponse>();
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Error response: {ErrorContent}", errorContent);
            throw new Exception($"Error fetching sets: {response.StatusCode}, {errorContent}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in SetsService.GetSetsAsync");
            // TODO: Need to have a fallback instead of throwing.
            throw;
        }
    }
}