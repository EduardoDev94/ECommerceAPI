# 🏗️ Estrutura do Core (Domain Layer)

## 📦 Organização do Projeto

```
Core/
├── Common/
│   └── Entity.cs                  ← Classe base para todas as entidades
│
├── Entities/                       ← Modelos de domínio
│   ├── User.cs
│   ├── Product.cs
│   ├── Cart.cs
│   ├── CartItem.cs
│   ├── Order.cs
│   ├── OrderItem.cs
│   ├── Coupon.cs
│   └── OrderStatus.cs             ← Enum para status
│
├── Repositories/                   ← Contratos (interfaces)
│   ├── IRepository.cs             ← Interface genérica base
│   ├── IUserRepository.cs
│   ├── IProductRepository.cs
│   ├── ICartRepository.cs
│   ├── ICartItemRepository.cs
│   ├── IOrderRepository.cs
│   ├── IOrderItemRepository.cs
│   └── ICouponRepository.cs
│
├── Domain.md                       ← Documentação de modelo
└── Core.csproj
```

## 🔗 Diagrama de Relacionamentos

```
User (1)
  ├── (1:1) Cart
  │   ├── (1:N) CartItems
  │   │   ├── (N:1) Product
  │   │   └── TotalPrice = Quantity × Product.Price
  │   └── (0..1) Coupon
  │       └── Validação: Active, NotExpired, WithinUsageLimit
  │
  └── (1:N) Orders
      └── (1:N) OrderItems
          ├── (N:1) Product (Reference Only)
          └── ⚠️ UnitPrice = Preço Capturado no Momento da Compra
                (Garante histórico consistente)

Product (N)
  ├── (N:1) CartItems (múltiplos carrinhos podem ter o mesmo produto)
  └── (N:1) OrderItems (múltiplos pedidos podem ter o mesmo produto)

Coupon (N)
  └── (N) Carts (múltiplos carrinhos podem usar o mesmo cupom)
```

## 📋 Resumo das Entidades

| Entidade | Propósito | Mutável? | Chave Estrangeira |
|----------|-----------|----------|------------------|
| **User** | Usuário do sistema | ✅ Sim | - |
| **Product** | Catálogo de produtos | ✅ Sim | - |
| **Cart** | Carrinho ativo | ✅ Sim | UserId, CouponId |
| **CartItem** | Item no carrinho | ✅ Sim | CartId, ProductId |
| **Order** | Pedido histórico | ❌ Não (Status apenas) | UserId |
| **OrderItem** | Item do pedido | ❌ Não | OrderId, ProductId |
| **Coupon** | Cupom de desconto | ✅ Sim | - |
| **OrderStatus** | Enum de status | - | - |

## 🎯 Padrões Implementados

### 1. **Entity Base Pattern**
Todas as entidades herdam de `Entity` que fornece:
- `Id` (Guid) - Identificador único
- `CreatedAt` (DateTime) - Data de criação
- `UpdatedAt` (DateTime) - Data de atualização

### 2. **Repository Pattern**
- `IRepository<T>` - Interface genérica com operações CRUD
- Interfaces especializadas por entidade com métodos customizados
- Promove abstração e testabilidade

### 3. **Domain-Driven Design (DDD)**
- Lógica de negócio nas entidades:
  - `Cart.CalculateTotals()` - Calcula totais com desconto
  - `Cart.AddItem()` - Adiciona item com validação
  - `Coupon.IsValid()` - Valida cupom
  - `OrderItem.CreateFromCartItem()` - Factory method
- Entidades imutáveis onde apropriado (Order é histórico)

### 4. **Separation of Concerns**
- **Entities**: Modelos de dados + Lógica de domínio
- **Repositories**: Abstração de persistência
- **Interfaces**: Contrato entre camadas

## 🔐 Regras de Negócio Críticas

### ✅ Validações

1. **Cupom deve ser válido para ser aplicado:**
   ```csharp
   - IsActive == true
   - DateTime.UtcNow <= ExpirationDate
   - TimesUsed < UsageLimit
   ```

2. **Order é imutável:**
   - Apenas `Status` pode ser alterado
   - Todos os dados são capturados no momento da compra

3. **OrderItem preserva histórico:**
   - `UnitPrice` é capturado no momento da compra
   - Garante consistência mesmo se Product.Price mude

4. **Cart calcula automaticamente:**
   - `TotalAmount` = Σ(CartItem.Quantity × Product.Price)
   - `DiscountAmount` = TotalAmount × (Coupon.DiscountPercentage / 100)
   - `FinalAmount` = TotalAmount - DiscountAmount

## 📚 Próximas Camadas

Após o **Core**, você implementará:

### **Application Layer**
- DTOs (Data Transfer Objects)
- Use Cases / Services
- Validators (FluentValidation)
- Mappers (AutoMapper)

### **Infrastructure Layer**
- EF Core DbContext
- Implementação dos Repositories
- Unit of Work Pattern
- Migrations

### **CrossCutting Layer**
- Middlewares (Logging, Correlational ID)
- Error Handling
- Exception Handling

### **API Layer**
- Controllers
- Endpoints
- Dependency Injection

## 🚀 Próximos Passos

1. ✅ Core (Domain) estruturado
2. ⏳ Implementar Application Layer
3. ⏳ Implementar Infrastructure com EF Core
4. ⏳ Adicionar validações com FluentValidation
5. ⏳ Configurar Middlewares
6. ⏳ Criar Controllers e Endpoints

---

**Desenvolvido com ❤️ usando .NET 10 + Clean Architecture**
