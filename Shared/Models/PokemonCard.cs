using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models;

[Table("cards")]
public class PokemonCard
{
    [Key]
    [Column("id")]
    public string Id { get; set; }
    
    [Column("name")]
    public string Name { get; set; }
    
    [Column("supertype")]
    public string SuperType { get; set; }
    
    [Column("level")]
    public int? Level { get; set; }
    
    [Column("hp")]
    public int? Hp { get; set; }
    
    [Column("evolves_from")]
    public string? EvolvesFrom { get; set; }
    
    [Column("retreat_cost")]
    public int? RetreatCost { get; set; }
    
    [Column("set_number")]
    public int? SetNumber { get; set; }
    
    [Column("artist")]
    public string? Artist { get; set; }
    
    [Column("rarity")]
    public string? Rarity { get; set; }
    
    [Column("flavor_text")]
    public string? FlavorText { get; set; }
    
    [Column("image_small")]
    public string ImageSmall { get; set; }
    
    [Column("image_large")]
    public string ImageLarge { get; set; }
    
    [Column("legality_unlimited")]
    public string LegalityUnlimited { get; set; }
    
    [Column("legality_standard")]
    public string? LegalityStandard { get; set; }
    
    [Column("legality_expanded")]
    public string? LegalityExpanded { get; set; }
}