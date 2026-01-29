using Shared.Models;

namespace BlazorPokemonCardSetViewer.Features.PokemonCard;

public class PokemonCardDataResponse
{
    public required string Id { get; init; }
    public string? Name { get; init; }
    public string? Hp { get; init; }
    public string? ImageSmall { get; init; }
    public string? ImageLarge { get; init; }
    public int? CardNumber { get; init; }
    public string? Raritity { get; init; }
}