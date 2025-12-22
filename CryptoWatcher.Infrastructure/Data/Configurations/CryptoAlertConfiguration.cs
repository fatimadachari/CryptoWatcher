using CryptoWatcher.Domain.Entities;
using CryptoWatcher.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CryptoWatcher.Infrastructure.Data.Configurations;

public class CryptoAlertConfiguration : IEntityTypeConfiguration<CryptoAlert>
{
    public void Configure(EntityTypeBuilder<CryptoAlert> builder)
    {
        builder.ToTable("CryptoAlerts");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.CryptoSymbol)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(a => a.TargetPrice)
            .IsRequired()
            .HasColumnType("decimal(18,2)"); // Precisão para valores monetários

        builder.Property(a => a.Condition)
            .IsRequired()
            .HasConversion<int>(); // Enum salvo como int no banco

        builder.Property(a => a.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.Property(a => a.UpdatedAt)
            .IsRequired(false);

        builder.Property(a => a.TriggeredAt)
            .IsRequired(false);

        // Índice composto para consultas rápidas de alertas ativos
        builder.HasIndex(a => new { a.Status, a.CryptoSymbol });
    }
}