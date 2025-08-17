using Shared.Models;

namespace Server.Features;

public class GetPokemonCardsResponse
{
    public required PokemonCardData[] Data { get; set; }
}