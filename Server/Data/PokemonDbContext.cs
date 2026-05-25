using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Server.Data;

public class PokemonDbContext : DbContext
{
    public DbSet<PokemonCard> PokemonCards { get; set; }
    public DbSet<PokemonSet> PokemonSets { get; set; }

    public PokemonDbContext(DbContextOptions<PokemonDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // modelBuilder.Entity<PokemonCard>().ToTable("cards");
        modelBuilder.Entity<PokemonCard>()
            .HasOne(pc => pc.Set)
            .WithOne()
            .HasForeignKey<PokemonCard>(pc => pc.SetId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<PokemonSet>().ToTable("sets");
    }
}
