namespace Shared.Models;

public class PokemonCard
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public string? Hp { get; init; }
    public CardImages? Images { get; init; }
}