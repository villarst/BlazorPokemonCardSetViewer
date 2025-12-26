using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Server.Data;

public class PokemonDbContext : DbContext
{
    public DbSet<PokemonCard> PokemonCards { get; set; }

    public PokemonDbContext(DbContextOptions<PokemonDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PokemonCard>().ToTable("cards");
    }
}