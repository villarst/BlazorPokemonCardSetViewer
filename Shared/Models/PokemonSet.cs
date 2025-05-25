namespace Shared.Models;

public class PokemonSet
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Series { get; init; }
    public required string Total { get; init; }
    public SetImages? Images { get; init; }
}