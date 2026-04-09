namespace Core.Entities;

using Core.Common;
using Core.Enums;

/// <summary>
/// Entidade de Usuário
/// Representa um usuário do sistema
/// 
/// Relacionamentos:
/// - 1:1 com Cart
/// - 1:N com Orders
/// </summary>
public class User : Entity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Customer;

    // Relacionamentos
    public Guid? CartId { get; set; }
    public virtual Cart? Cart { get; set; }
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
