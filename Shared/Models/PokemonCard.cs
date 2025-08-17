namespace Shared.Models;

public class PokemonCard
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Supertype { get; set; } = string.Empty;
    public string? Subtypes { get; set; } // JSON
    public string? Hp { get; set; }
    public string? Types { get; set; } // JSON
    public string? EvolvesTo { get; set; } // JSON
    public string? SetId { get; set; }
    public string? SetName { get; set; }
    public string? Number { get; set; }
    public string? Artist { get; set; }
    public string? Rarity { get; set; }
    public string? FlavorText { get; set; }
    public string? NationalPokedexNumbers { get; set; } // JSON
    public string? RegulationMark { get; set; }
    public string? SmallImage { get; set; }
    public string? LargeImage { get; set; }
    public string? RawJson { get; set; } // Complete JSON
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}