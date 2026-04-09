using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Configuração da entidade Coupon
/// Representa um cupom de desconto
/// Validações importantes:
/// - Deve estar ativo (IsActive = true)
/// - Não deve estar expirado (ExpirationDate >= DateTime.UtcNow)
/// - Deve estar dentro do limite de uso (TimesUsed < UsageLimit)
/// </summary>
public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Code)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnType("varchar(50)");

        builder.Property(c => c.DiscountPercentage)
            .HasPrecision(5, 2)
            .IsRequired()
            .HasColumnType("numeric(5,2)");

        builder.Property(c => c.ExpirationDate)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.Property(c => c.IsActive)
            .HasDefaultValue(true);

        builder.Property(c => c.UsageLimit)
            .HasDefaultValue(int.MaxValue)
            .HasColumnType("integer");

        builder.Property(c => c.TimesUsed)
            .HasDefaultValue(0)
            .HasColumnType("integer");

        // Propriedades de auditoria
        builder.Property(c => c.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.Property(c => c.UpdatedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        // Índices para melhor performance
        builder.HasIndex(c => c.Code)
            .IsUnique()
            .HasDatabaseName("idx_coupon_code_unique");

        builder.HasIndex(c => c.IsActive)
            .HasDatabaseName("idx_coupon_is_active");

        builder.HasIndex(c => c.ExpirationDate)
            .HasDatabaseName("idx_coupon_expiration_date");

        builder.HasIndex(c => new { c.Code, c.IsActive })
            .HasDatabaseName("idx_coupon_code_active");

        // Nome da tabela
        builder.ToTable("Coupons", schema: "public");

        // Comentários para documentação do banco
        builder.HasComment("Tabela de cupons de desconto");

        // Dados iniciais
        SeedData(builder);
    }

    /// <summary>
    /// Popula dados iniciais de cupons
    /// </summary>
    private static void SeedData(EntityTypeBuilder<Coupon> builder)
    {
        builder.HasData(
            new Coupon
            {
                Id = new Guid("a1b2c3d4-e5f6-47a8-b9c0-d1e2f3a4b5c6"),
                Code = "WELCOME10",
                DiscountPercentage = 10m,
                ExpirationDate = new DateTime(2027, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true,
                UsageLimit = 100,
                TimesUsed = 0,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Coupon
            {
                Id = new Guid("b2c3d4e5-f6a7-48b9-c0d1-e2f3a4b5c6d7"),
                Code = "PROMO20",
                DiscountPercentage = 20m,
                ExpirationDate = new DateTime(2027, 6, 1, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true,
                UsageLimit = 50,
                TimesUsed = 0,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Coupon
            {
                Id = new Guid("c3d4e5f6-a7b8-49ca-d1e2-f3a4b5c6d7e8"),
                Code = "VIP15",
                DiscountPercentage = 15m,
                ExpirationDate = new DateTime(2027, 12, 31, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true,
                UsageLimit = int.MaxValue,
                TimesUsed = 0,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
