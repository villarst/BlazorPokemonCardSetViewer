using Shared.Models;

namespace BlazorPokemonCardSetViewer.Contracts;

public interface ICardsService
{
    Task<IEnumerable<PokemonCard>> GetCardsAsync(string searchTerm);
}