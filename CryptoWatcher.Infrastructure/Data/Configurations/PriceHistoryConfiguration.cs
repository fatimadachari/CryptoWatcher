using CryptoWatcher.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoWatcher.Infrastructure.Data.Configurations;

public class PriceHistoryConfiguration : IEntityTypeConfiguration<PriceHistory>
{
    public void Configure(EntityTypeBuilder<PriceHistory> builder)
    {
        builder.ToTable("PriceHistories");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.CryptoSymbol)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(p => p.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.Source)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.FetchedAt)
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        // Índice para consultas por símbolo e data
        builder.HasIndex(p => new { p.CryptoSymbol, p.FetchedAt });
    }
}