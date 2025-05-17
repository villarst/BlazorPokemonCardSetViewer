using Shared.Models;

namespace Server.Features;

public class GetPokemonCardResponse
{
    public required PokemonCard Data { get; set; }
}