using System.Text.Json;
using BlazorPokemonCardSetViewer.Contracts;
using ReactiveUI;
using Server.Features;
using Shared.Models;

namespace BlazorPokemonCardSetViewer.Services;

public class CardService :  ReactiveObject, ICardService
{
    private readonly HttpClient _httpClient;
    private const string ApiKey = "15625e63-354d-4ce5-a221-a5c200ce57f4";

    public CardService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://api.pokemontcg.io/v2/");
        _httpClient.DefaultRequestHeaders.Add("X-Api-Key", ApiKey);
    }
    
    public async Task<PokemonCard> GetCardAsync(string cardId)
    {
        var response = await _httpClient.GetAsync($"cards/{cardId}");
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var cardResponse = JsonSerializer.Deserialize<PokemonCardResponse>(content, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return cardResponse.Data;
        }
        
        throw new Exception($"Error fetching card: {response.StatusCode}");
    }
}