using Core.Entities;
using Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Configuração da entidade User
/// Relacionamentos:
/// - 1:1 com Cart (usuário sempre tem um carrinho)
/// - 1:N com Orders (usuário pode ter múltiplos pedidos)
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnType("varchar(100)");

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnType("varchar(100)");

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnType("varchar(200)");

        builder.Property(u => u.Role)
            .IsRequired()
            .HasDefaultValue(UserRole.Customer)
            .HasConversion<string>()
            .HasColumnType("varchar(20)");

        // Índice único para Email
        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("idx_user_email_unique");

        // Propriedades de auditoria
        builder.Property(u => u.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.Property(u => u.UpdatedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.Property(u => u.IsActive)
            .HasDefaultValue(true);

        // Relacionamento 1:1 com Cart
        builder.HasOne(u => u.Cart)
            .WithOne(c => c.User)
            .HasForeignKey<Cart>(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relacionamento 1:N com Orders
        builder.HasMany(u => u.Orders)
            .WithOne(o => o.User)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Nome da tabela
        builder.ToTable("Users", schema: "public");

        // Comentários para documentação do banco
        builder.HasComment("Tabela de usuários do sistema");
    }
}
