using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models;

[Table("sets")]
public class PokemonSet
{
    [Key]
    [Column("id")]
    public string Id { get; set; }
    
    [Column("name")]
    public string Name { get; set; }
    
    [Column("series")]
    public string Series { get; set; }

    [Column("printed_total")]
    public int PrintedTotal { get; set; }

    [Column("total")]
    public int Total { get; set; }

    [Column("ptcgo_code")]
    public string? PtcgoCode { get; set; }

    [Column("release_date")]
    public string ReleaseDate { get; set; }

    [Column("updated_at")]
    public string UpdatedAt { get; set; }

    [Column("image_symbol")]
    public string ImageSymbol { get; set; }

    [Column("image_logo")]
    public string ImageLogo { get; set; }

    [Column("legality_unlimited")]
    public string LegalityUnlimited { get; set; }

    [Column("legality_standard")]
    public string? LegalityStandard { get; set; }
    
    [Column("legality_expanded")]
    public string? LegalityExpanded { get; set; }
}