using Shared.Models;

namespace BlazorPokemonCardSetViewer.Features.PokemonCard;

public class PokemonCardDataResponse
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string SuperType { get; set; }
    public int? Level { get; set; }
    public int? Hp { get; init; }
    public string? EvolvesFrom { get; set; }
    public int? RetreatCost { get; set; }
    public int? SetNumber { get; set; }
    public string? Artist { get; set; }
    public string? FlavorText { get; set; }
    public required string ImageSmall { get; init; }
    public required string ImageLarge { get; init; }
    public int? CardNumber { get; init; }
    public string? Rarity { get; set; }
    public required string LegalityUnlimited { get; set; }
    public string? LegalityStandard { get; set; }
    public string? LegalityExpanded { get; set; }
}