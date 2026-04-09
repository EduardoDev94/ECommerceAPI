using Core.Entities;
using Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Configuração da entidade Order
/// Representa um pedido finalizado (histórico de compra)
/// IMPORTANTE: Order é IMUTÁVEL após criação. Apenas Status pode ser alterado.
/// Relacionamentos:
/// - N:1 com User (cada pedido pertence a um usuário)
/// - 1:N com OrderItems (pedido contém múltiplos itens)
/// </summary>
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.UserId)
            .IsRequired();

        builder.Property(o => o.OrderDate)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.Property(o => o.TotalAmount)
            .HasPrecision(18, 2)
            .IsRequired()
            .HasColumnType("numeric(18,2)");

        builder.Property(o => o.DiscountAmount)
            .HasPrecision(18, 2)
            .HasDefaultValue(0)
            .HasColumnType("numeric(18,2)");

        builder.Property(o => o.FinalAmount)
            .HasPrecision(18, 2)
            .IsRequired()
            .HasColumnType("numeric(18,2)");

        builder.Property(o => o.CouponCode)
            .HasMaxLength(50)
            .HasColumnType("varchar(50)");

        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(OrderStatus.Pending)
            .HasColumnType("integer");

        // Propriedades de auditoria
        builder.Property(o => o.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.Property(o => o.UpdatedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.Property(o => o.IsActive)
            .HasDefaultValue(true);

        // Relacionamento 1:N com OrderItems
        builder.HasMany(o => o.Items)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índices para melhor performance
        builder.HasIndex(o => o.UserId)
            .HasDatabaseName("idx_order_userid");

        builder.HasIndex(o => o.OrderDate)
            .HasDatabaseName("idx_order_orderdate");

        builder.HasIndex(o => o.Status)
            .HasDatabaseName("idx_order_status");

        builder.HasIndex(o => new { o.UserId, o.OrderDate })
            .HasDatabaseName("idx_order_userid_orderdate");

        // Nome da tabela
        builder.ToTable("Orders", schema: "public");

        // Comentários para documentação do banco
        builder.HasComment("Tabela de pedidos finalizados - IMUTÁVEL após criação");
    }
}
