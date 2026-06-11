using Microsoft.EntityFrameworkCore;
using TradingBot.Core.Entities;

namespace TradingBot.Core.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<MarketData> MarketData => Set<MarketData>();

    public DbSet<Signal> Signals => Set<Signal>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MarketData>((entity) =>
        {
            entity.ToTable("market_data");
            entity.HasKey((marketData) => marketData.Id);
            entity.Property((marketData) => marketData.Id).HasColumnName("id");
            entity.Property((marketData) => marketData.Symbol).HasColumnName("symbol").HasMaxLength(32).IsRequired();
            entity.Property((marketData) => marketData.Market).HasColumnName("market").HasMaxLength(64).IsRequired();
            entity.Property((marketData) => marketData.Open).HasColumnName("open").HasPrecision(18, 6);
            entity.Property((marketData) => marketData.High).HasColumnName("high").HasPrecision(18, 6);
            entity.Property((marketData) => marketData.Low).HasColumnName("low").HasPrecision(18, 6);
            entity.Property((marketData) => marketData.Close).HasColumnName("close").HasPrecision(18, 6);
            entity.Property((marketData) => marketData.Price).HasColumnName("price").HasPrecision(18, 6);
            entity.Property((marketData) => marketData.Volume).HasColumnName("volume").HasPrecision(20, 4);
            entity.Property((marketData) => marketData.Source).HasColumnName("source").HasMaxLength(64).IsRequired();
            entity.Property((marketData) => marketData.Timestamp).HasColumnName("timestamp");
            entity.Property((marketData) => marketData.CreatedAt).HasColumnName("created_at");
            entity.HasIndex((marketData) => new { marketData.Symbol, marketData.Timestamp });
        });

        modelBuilder.Entity<Signal>((entity) =>
        {
            entity.ToTable("signals");
            entity.HasKey((signal) => signal.Id);
            entity.Property((signal) => signal.Id).HasColumnName("id");
            entity.Property((signal) => signal.Symbol).HasColumnName("symbol").HasMaxLength(32).IsRequired();
            entity.Property((signal) => signal.Market).HasColumnName("market").HasMaxLength(64).IsRequired();
            entity.Property((signal) => signal.Recommendation).HasColumnName("recommendation").HasMaxLength(16).IsRequired();
            entity.Property((signal) => signal.Confidence).HasColumnName("confidence").HasPrecision(8, 4);
            entity.Property((signal) => signal.EntryPrice).HasColumnName("entry_price").HasPrecision(18, 6);
            entity.Property((signal) => signal.StopLoss).HasColumnName("stop_loss").HasPrecision(18, 6);
            entity.Property((signal) => signal.TargetPrice).HasColumnName("target_price").HasPrecision(18, 6);
            entity.Property((signal) => signal.RiskRewardRatio).HasColumnName("risk_reward_ratio").HasPrecision(8, 4);
            entity.Property((signal) => signal.Rationale).HasColumnName("rationale").HasMaxLength(1024).IsRequired();
            entity.Property((signal) => signal.Source).HasColumnName("source").HasMaxLength(64).IsRequired();
            entity.Property((signal) => signal.CreatedAt).HasColumnName("created_at");
            entity.HasIndex((signal) => new { signal.Symbol, signal.CreatedAt });
        });
    }
}
