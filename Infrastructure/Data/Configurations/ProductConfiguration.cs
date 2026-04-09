using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Configuração da entidade Product
/// Relacionamentos:
/// - 1:N com CartItems (produto pode estar em múltiplos carrinhos)
/// - 1:N com OrderItems (produto pode estar em múltiplos pedidos)
/// </summary>
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnType("varchar(200)");

        builder.Property(p => p.Description)
            .HasMaxLength(1000)
            .HasColumnType("text");

        builder.Property(p => p.Price)
            .HasPrecision(18, 2)
            .IsRequired()
            .HasColumnType("numeric(18,2)");

        builder.Property(p => p.Stock)
            .IsRequired()
            .HasDefaultValue(0)
            .HasColumnType("integer");

        // Propriedades de auditoria
        builder.Property(p => p.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.Property(p => p.UpdatedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.Property(p => p.IsActive)
            .HasDefaultValue(true);

        // Índices para melhor performance
        builder.HasIndex(p => p.Name)
            .HasDatabaseName("idx_product_name");

        builder.HasIndex(p => p.IsActive)
            .HasDatabaseName("idx_product_is_active");

        // Nome da tabela
        builder.ToTable("Products", schema: "public");

        // Comentários para documentação do banco
        builder.HasComment("Tabela de produtos disponíveis no catálogo");
    }
}
