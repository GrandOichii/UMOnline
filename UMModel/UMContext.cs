using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using UMModel.Models;

namespace UMModel;

public class UMContext : DbContext
{
    public UMContext() : base() {}
    public UMContext(DbContextOptions<UMContext> options) : base(options) {}

    public DbSet<Loadout> Loadouts { get; set; }
    public DbSet<Fighter> Fighters { get; set; }
    public DbSet<Card> Cards { get; set; }
    public DbSet<ContentUpdate> ContentUpdates { get; set; }
    public DbSet<CoreScript> CoreScripts { get; set; }
    public DbSet<MatchConfig> Configs { get; set; }

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

        modelBuilder.Entity<ContentUpdate>()
            .HasKey(cu => cu.Id);
        modelBuilder.Entity<ContentUpdate>()
            .Property(cu => cu.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<MatchConfig>()
            .HasKey(s => s.Name);
        modelBuilder.Entity<MatchConfig>()
            .HasData(MatchConfig.GetDefaultData());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        optionsBuilder.UseNpgsql(configuration.GetConnectionString("UMContext"));
    }
}