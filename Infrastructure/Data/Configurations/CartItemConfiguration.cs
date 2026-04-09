using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Configuração da entidade CartItem
/// Representa um item dentro de um carrinho
/// Relacionamentos:
/// - N:1 com Cart
/// - N:1 com Product
/// </summary>
public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.HasKey(ci => ci.Id);

        builder.Property(ci => ci.CartId)
            .IsRequired();

        builder.Property(ci => ci.ProductId)
            .IsRequired();

        builder.Property(ci => ci.Quantity)
            .IsRequired()
            .HasDefaultValue(1)
            .HasColumnType("integer");

        // Propriedades de auditoria
        builder.Property(ci => ci.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.Property(ci => ci.UpdatedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.Property(ci => ci.IsActive)
            .HasDefaultValue(true);

        // Relacionamento N:1 com Product
        builder.HasOne(ci => ci.Product)
            .WithMany(p => p.CartItems)
            .HasForeignKey(ci => ci.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índice composto para evitar duplicação de itens no mesmo carrinho
        builder.HasIndex(ci => new { ci.CartId, ci.ProductId })
            .IsUnique()
            .HasDatabaseName("idx_cartitem_cart_product_unique");

        builder.HasIndex(ci => ci.CartId)
            .HasDatabaseName("idx_cartitem_cartid");

        // Nome da tabela
        builder.ToTable("CartItems", schema: "public");

        // Comentários para documentação do banco
        builder.HasComment("Tabela de itens dentro dos carrinhos");
    }
}
