using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using UMModel.Models;

namespace UMModel;

public class UMContext : DbContext
{
    public DbSet<Loadout> Loadouts { get; set; }
    public DbSet<Fighter> Fighters { get; set; }
    public DbSet<Card> Cards { get; set; }
    public DbSet<CoreScript> CoreScripts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        optionsBuilder.UseNpgsql(config.GetConnectionString("UMContext"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Loadout>()
            .HasKey(l => l.Name);

        modelBuilder.Entity<Fighter>()
            .HasKey(f => f.Key);
            
        modelBuilder.Entity<Card>()
            .HasKey(f => f.Key);

        modelBuilder.Entity<CoreScript>()
            .Property(s => s.Id)
            .ValueGeneratedOnAdd();
            
        modelBuilder.Entity<CoreScript>()
            .HasKey(s => s.Id);
    }
}