using Core.Entities;
using Core.Common;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Data.Configurations;

namespace Infrastructure.Data;

/// <summary>
/// DbContext principal da aplicação E-commerce
/// Responsável pela configuração e mapeamento de todas as entidades
/// Segue a arquitetura Clean Architecture
/// </summary>
public class ECommerceDbContext : DbContext
{
    public ECommerceDbContext(DbContextOptions<ECommerceDbContext> options) 
        : base(options)
    {
    }

    #region DbSets - Coleções de Entidades

    /// <summary>
    /// Tabela de usuários do sistema
    /// </summary>
    public DbSet<User> Users { get; set; } = null!;

    /// <summary>
    /// Tabela de produtos disponíveis no catálogo
    /// </summary>
    public DbSet<Product> Products { get; set; } = null!;

    /// <summary>
    /// Tabela de carrinhos de compras
    /// </summary>
    public DbSet<Cart> Carts { get; set; } = null!;

    /// <summary>
    /// Tabela de itens dentro dos carrinhos
    /// </summary>
    public DbSet<CartItem> CartItems { get; set; } = null!;

    /// <summary>
    /// Tabela de pedidos finalizados (histórico de compra)
    /// </summary>
    public DbSet<Order> Orders { get; set; } = null!;

    /// <summary>
    /// Tabela de itens dentro dos pedidos
    /// </summary>
    public DbSet<OrderItem> OrderItems { get; set; } = null!;

    /// <summary>
    /// Tabela de cupons de desconto
    /// </summary>
    public DbSet<Coupon> Coupons { get; set; } = null!;

    #endregion

    /// <summary>
    /// Configuração do modelo via Fluent API
    /// Aplica todas as configurações de entidades
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplicar configurações de entidades
        // Cada entidade tem sua própria classe de configuração
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        modelBuilder.ApplyConfiguration(new CartConfiguration());
        modelBuilder.ApplyConfiguration(new CartItemConfiguration());
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        modelBuilder.ApplyConfiguration(new OrderItemConfiguration());
        modelBuilder.ApplyConfiguration(new CouponConfiguration());
    }

    #region Override Methods - Gerenciamento de Timestamps

    /// <summary>
    /// Override para salvar automaticamente CreatedAt e UpdatedAt
    /// Sincronamente
    /// </summary>
    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    /// <summary>
    /// Override para salvar automaticamente CreatedAt e UpdatedAt
    /// Assincronamente
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Atualiza os timestamps de auditoria (CreatedAt e UpdatedAt)
    /// para todas as entidades que herdam de Entity
    /// </summary>
    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is Entity);

        foreach (var entry in entries)
        {
            var entity = (Entity)entry.Entity;

            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTime.UtcNow;
                entity.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }

    #endregion
}
