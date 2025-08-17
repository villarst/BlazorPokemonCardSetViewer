using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Server.Data;

public class PokemonDbContext : DbContext
{
    public DbSet<PokemonCard> PokemonCards { get; set; }

    public PokemonDbContext(DbContextOptions<PokemonDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PokemonCard>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.SetId);
            entity.HasIndex(e => e.Supertype);
            entity.HasIndex(e => e.Rarity);
            
            // Make sure EF knows these are just regular string columns
            entity.Property(e => e.SmallImage).HasMaxLength(500);
            entity.Property(e => e.LargeImage).HasMaxLength(500);
        });
    }
}