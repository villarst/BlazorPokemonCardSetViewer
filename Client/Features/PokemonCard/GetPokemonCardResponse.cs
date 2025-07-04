using Shared.Models;

namespace Server.Features;
public class GetPokemonCardResponse
{
    public required PokemonCardData Data { get; set; }
}

public class PokemonCardData
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public string? Hp { get; init; }
    public required CardImages Images { get; init; }
}