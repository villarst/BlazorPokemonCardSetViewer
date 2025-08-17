using Shared.Models;

namespace BlazorPokemonCardSetViewer.Contracts;

public interface ICardsService
{
    Task<PagedList<PokemonCard>> GetCardsAsync(PagedRequest request);
    Task<IEnumerable<PokemonCard>> GetAllCardsAsync(string searchTerm); // Keep if needed
}