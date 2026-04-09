using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Configuração da entidade OrderItem
/// Representa um item dentro de um pedido
/// CRÍTICO: UnitPrice armazena o valor do produto NO MOMENTO DA COMPRA
/// Isso garante consistência histórica, mesmo que o preço do produto mude depois
/// Relacionamentos:
/// - N:1 com Order
/// - N:1 com Product (somente referência)
/// </summary>
public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(oi => oi.Id);

        builder.Property(oi => oi.OrderId)
            .IsRequired();

        builder.Property(oi => oi.ProductId)
            .IsRequired();

        builder.Property(oi => oi.Quantity)
            .IsRequired()
            .HasColumnType("integer");

        builder.Property(oi => oi.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired()
            .HasColumnType("numeric(18,2)");

        // Propriedades de auditoria
        builder.Property(oi => oi.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.Property(oi => oi.UpdatedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.Property(oi => oi.IsActive)
            .HasDefaultValue(true);

        // Relacionamento N:1 com Product
        builder.HasOne(oi => oi.Product)
            .WithMany(p => p.OrderItems)
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices
        builder.HasIndex(oi => oi.OrderId)
            .HasDatabaseName("idx_orderitem_orderid");

        builder.HasIndex(oi => oi.ProductId)
            .HasDatabaseName("idx_orderitem_productid");

        builder.HasIndex(oi => new { oi.OrderId, oi.ProductId })
            .HasDatabaseName("idx_orderitem_order_product");

        // Nome da tabela
        builder.ToTable("OrderItems", schema: "public");

        // Comentários para documentação do banco
        builder.HasComment("Tabela de itens dentro dos pedidos - Armazena SNAPSHOT do preço no momento da compra");
    }
}
