using Shared.Models;

namespace BlazorPokemonCardSetViewer.Contracts;

public interface ISetsService
{
    // Do not know what to implement yet. 
    Task<PokemonSet?> GetSetAsync();
}