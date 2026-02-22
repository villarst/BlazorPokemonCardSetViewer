namespace BlazorPokemonCardSetViewer.Features.PokemonSet;

public class PokemonSetDataResponse
{
    public required string Id { get; init; }
    
    public string? Name { get; init; }
    
    public string? Series { get; init; }

    public int? PrintedTotal { get; init; }

    public int? Total { get; init; }

    public string? PtcgoCode { get; init; }

    public DateTime? ReleaseDate { get; init; }

    public string? UpdatedAt { get; init; }

    public string? ImageSymbol { get; init; }

    public string? ImageLogo { get; init; }

    public string? LegalityUnlimited { get; init; }

    public string? LegalityStandard { get; init; }
    
    public string? LegalityExpanded { get; init; }
}