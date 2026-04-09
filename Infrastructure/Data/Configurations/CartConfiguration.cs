using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Configuração da entidade Cart
/// Relacionamentos:
/// - N:1 com User (cada carrinho pertence a um usuário)
/// - 1:N com CartItems (carrinho contém múltiplos itens)
/// - 0..1 com Coupon (carrinho pode ter um cupom opcional)
/// </summary>
public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.UserId)
            .IsRequired();

        builder.Property(c => c.TotalAmount)
            .HasPrecision(18, 2)
            .HasDefaultValue(0)
            .HasColumnType("numeric(18,2)");

        builder.Property(c => c.DiscountAmount)
            .HasPrecision(18, 2)
            .HasDefaultValue(0)
            .HasColumnType("numeric(18,2)");

        builder.Property(c => c.FinalAmount)
            .HasPrecision(18, 2)
            .HasDefaultValue(0)
            .HasColumnType("numeric(18,2)");

        builder.Property(c => c.CouponId)
            .HasColumnType("uuid");

        // Propriedades de auditoria
        builder.Property(c => c.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.Property(c => c.UpdatedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.Property(c => c.IsActive)
            .HasDefaultValue(true);

        // Relacionamento 1:N com CartItems
        builder.HasMany(c => c.Items)
            .WithOne(ci => ci.Cart)
            .HasForeignKey(ci => ci.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relacionamento 0..1 com Coupon
        builder.HasOne(c => c.Coupon)
            .WithMany(cu => cu.Carts)
            .HasForeignKey(c => c.CouponId)
            .OnDelete(DeleteBehavior.SetNull);

        // Índices
        builder.HasIndex(c => c.UserId)
            .IsUnique()
            .HasDatabaseName("idx_cart_userid_unique");

        builder.HasIndex(c => c.IsActive)
            .HasDatabaseName("idx_cart_is_active");

        // Nome da tabela
        builder.ToTable("Carts", schema: "public");

        // Comentários para documentação do banco
        builder.HasComment("Tabela de carrinhos de compras dos usuários");
    }
}
