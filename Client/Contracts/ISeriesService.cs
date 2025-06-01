using Shared.Models;

namespace BlazorPokemonCardSetViewer.Contracts;

public interface ISeriesService
{
    // Do not know what to implement yet. 
    Task<List<PokemonSet>?> GetSeriesAsync(string seriesName);
}