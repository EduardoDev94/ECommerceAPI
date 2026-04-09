# 📂 Infrastructure Project Structure

## 🏗️ Visão Geral

A camada **Infrastructure** é responsável por toda a implementação técnica de persistência de dados e acesso ao banco.

```
Infrastructure/
│
├── 📂 Data/                              # Camada de Acesso a Dados
│   ├── 📄 ECommerceDbContext.cs         # DbContext principal (coração da aplicação)
│   │
│   ├── 📂 Configurations/                # Configurações do EF Core (Fluent API)
│   │   ├── 📄 UserConfiguration.cs
│   │   ├── 📄 ProductConfiguration.cs
│   │   ├── 📄 CartConfiguration.cs
│   │   ├── 📄 CartItemConfiguration.cs
│   │   ├── 📄 OrderConfiguration.cs
│   │   ├── 📄 OrderItemConfiguration.cs
│   │   ├── 📄 CouponConfiguration.cs
│   │   └── 📄 README.md
│   │
│   ├── 📂 Repositories/                  # Implementações do Repository Pattern
│   │   ├── 📄 Repository.cs              # Generic base repository
│   │   ├── 📄 UserRepository.cs
│   │   ├── 📄 ProductRepository.cs
│   │   ├── 📄 CartRepository.cs
│   │   ├── 📄 CartItemRepository.cs
│   │   ├── 📄 OrderRepository.cs
│   │   ├── 📄 OrderItemRepository.cs
│   │   ├── 📄 CouponRepository.cs
│   │   └── 📄 UnitOfWork.cs              # Coordena todos os repositórios
│   │
│   └── 📂 Migrations/                    # EF Core Migrations (Auto-geradas)
│       ├── 📄 [timestamp]_InitialCreate.cs
│       └── 📄 ECommerceDbContextModelSnapshot.cs
│
├── 📂 Services/                          # Serviços técnicos (opcional)
│   └── 📄 DatabaseSeeder.cs
│
├── 📂 Extensions/                        # Métodos de extensão para DI
│   └── 📄 ServiceCollectionExtensions.cs
│
├── 📄 Infrastructure.csproj              # Project file
└── 📄 README.md                          # Documentação do projeto
```

---

## 🔍 Descrição de Cada Componente

### **📂 Data/**
Contém toda a configuração e acesso ao banco de dados.

#### **ECommerceDbContext.cs**
- DbContext principal da aplicação
- Registra todos os DbSets (User, Product, Cart, etc)
- Aplica configurações de entidades
- Gerencia timestamps automaticamente
- Override de SaveChanges para auditoria

#### **📂 Configurations/**
Cada arquivo configura UMA entidade usando Fluent API.
```csharp
// Padrão IEntityTypeConfiguration<T>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder) { ... }
}
```

**Benefícios:**
- ✅ Modular e escalável
- ✅ Fácil de manter
- ✅ Testável
- ✅ Reutilizável

#### **📂 Repositories/**
Implementação do **Repository Pattern** com **Unit of Work**.

```csharp
// Generic repository
public class Repository<T> : IRepository<T> { ... }

// Repositórios especializados
public class UserRepository : Repository<User>, IUserRepository { ... }

// Coordenador central
public class UnitOfWork : IDisposable { ... }
```

#### **📂 Migrations/**
Gerados automaticamente pelo EF Core.
```bash
dotnet ef migrations add MigrationName
dotnet ef database update
```

---

### **📂 Services/**
Serviços técnicos de infraestrutura (não confundir com Application Services).

```csharp
// Exemplo: Popular dados iniciais
public class DatabaseSeeder
{
    public async Task SeedAsync(ECommerceDbContext context)
    {
        if (!context.Users.Any())
        {
            // Adicionar dados iniciais
        }
    }
}
```

---

### **📂 Extensions/**
Extensões para registrar serviços de infraestrutura no DI Container.

```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Registrar DbContext
        services.AddDbContext<ECommerceDbContext>(options => ...);
        
        // Registrar Repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        
        // Registrar Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        return services;
    }
}
```

**Uso em Program.cs:**
```csharp
builder.Services.AddInfrastructure(builder.Configuration);
```

---

## 📊 Fluxo de Dados

```
┌─────────────────────────────────────────┐
│        API Controller (HTTP)            │
└────────────────┬────────────────────────┘
                 │
┌─────────────────▼────────────────────────┐
│   Application Service (Business Logic)   │
└────────────────┬────────────────────────┘
                 │
┌─────────────────▼────────────────────────┐
│    Repository (IRepository<T>)           │
└────────────────┬────────────────────────┘
                 │
┌─────────────────▼────────────────────────┐
│ ECommerceDbContext (EF Core)             │
└────────────────┬────────────────────────┘
                 │
┌─────────────────▼────────────────────────┐
│  PostgreSQL Database                    │
└─────────────────────────────────────────┘
```

---

## 🔐 Padrões Implementados

### **1. Repository Pattern**
Abstração de acesso a dados. Todas as queries vão através de repositórios.

```csharp
interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetActiveUsersAsync();
}
```

### **2. Unit of Work Pattern**
Coordena múltiplos repositórios em uma transação.

```csharp
public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IProductRepository Products { get; }
    IOrderRepository Orders { get; }
    
    Task<int> SaveChangesAsync();
}
```

### **3. Dependency Injection**
Registra serviços no container de DI.

```csharp
services.AddScoped<IUnitOfWork, UnitOfWork>();
services.AddScoped<DatabaseSeeder>();
```

---

## 🚀 Workflow Típico

### **1. Criar uma Nova Entidade**
```csharp
// 1. Core/Entities/Category.cs
public class Category : Entity { ... }

// 2. Core/Repositories/ICategoryRepository.cs
public interface ICategoryRepository : IRepository<Category> { ... }

// 3. Infrastructure/Data/Configurations/CategoryConfiguration.cs
public class CategoryConfiguration : IEntityTypeConfiguration<Category> { ... }

// 4. Infrastructure/Data/Repositories/CategoryRepository.cs
public class CategoryRepository : Repository<Category>, ICategoryRepository { ... }

// 5. Registrar em UnitOfWork
public IRepositor<Category> Categories { get; }

// 6. Criar migration
dotnet ef migrations add AddCategory
dotnet ef database update
```

---

## 📋 Checklist - Pronto para Uso

- ✅ ECommerceDbContext configurado
- ✅ 7 Configurações de entidades (User, Product, Cart, CartItem, Order, OrderItem, Coupon)
- ✅ Índices e constraints definidos
- ✅ Auditoria automatizada (CreatedAt, UpdatedAt)
- ✅ Seed data (cupons iniciais)
- ⭕ Repositories implementados
- ⭕ Unit of Work implementado
- ⭕ Database seeder
- ⭕ Extensions para DI

---

## 💾 Configuração PostgreSQL

```csharp
// appsettings.json
"ConnectionStrings": {
    "DefaultConnection": "Host=postgres-ecommerce;Port=5432;Database=ECommerceDb;Username=ecommerce_user;Password=SecurePassword123!"
}

// Program.cs
services.AddDbContext<ECommerceDbContext>(options =>
    options.UseNpgsql(connectionString)
);
```

---

## 🛠️ Comandos EF Core Úteis

```bash
# Criar migration
dotnet ef migrations add MigrationName \
  --project ..\Infrastructure\Infrastructure.csproj \
  --startup-project .

# Atualizar banco
dotnet ef database update \
  --project ..\Infrastructure\Infrastructure.csproj \
  --startup-project .

# Remover última migration
dotnet ef migrations remove \
  --project ..\Infrastructure\Infrastructure.csproj

# Gerar script SQL
dotnet ef migrations script \
  --project ..\Infrastructure\Infrastructure.csproj \
  > script.sql
```

---

## 📚 Referências

- [Entity Framework Core Documentation](https://learn.microsoft.com/en-us/ef/core/)
- [Repository Pattern](https://martinfowler.com/eaaCatalog/repository.html)
- [Unit of Work Pattern](https://martinfowler.com/eaaCatalog/unitOfWork.html)
- [PostgreSQL with EF Core](https://www.npgsql.org/efcore/)

---

**Status**: 🟢 DbContext & Configurations completo | 🔴 Repositories pendente

**Desenvolvido com ❤️ usando .NET 10 + Clean Architecture**
