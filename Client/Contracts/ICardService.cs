using Shared.Models;

namespace BlazorPokemonCardSetViewer.Contracts;

public interface ICardService
{
    Task<PokemonCard> GetCardAsync(string cardId);
}