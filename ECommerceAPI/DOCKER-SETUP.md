# 🐳 Docker Compose - E-commerce API

## 📋 Pré-requisitos

- Docker Desktop instalado
- Docker Compose (incluído no Docker Desktop)
- .NET 10 SDK (para desenvolvimento local)

## 🚀 Como Iniciar

### 1. **Via Docker Compose** (Recomendado)

```bash
# Ir para o diretório raiz do projeto
cd C:\Users\USER\source\repos\EcommerceAPI\ECommerceAPI

# Fazer build e iniciar todos os containers
docker-compose up -d

# Ver logs da aplicação
docker-compose logs -f ecommerce-api

# Parar todos os containers
docker-compose down

# Parar e remover volumes (limpar dados)
docker-compose down -v
```

### 2. **Localmente** (Desenvolvimento)

```bash
# Restaurar dependências
dotnet restore

# Aplicar migrations
dotnet ef database update --project ..\Infrastructure\Infrastructure.csproj --startup-project .

# Executar a aplicação
dotnet run

# A aplicação iniciará em:
# HTTP: http://localhost:5251
# HTTPS: https://localhost:7188
```

## 📊 Acessar Serviços

| Serviço | URL | Credenciais |
|---------|-----|-----------|
| **API - Swagger/Scalar** | http://localhost:8080/scalar/v1 | - |
| **API - OpenAPI JSON** | http://localhost:8080/openapi/v1.json | - |
| **pgAdmin** | http://localhost:5050 | admin@ecommerce.com / admin123 |
| **PostgreSQL** | localhost:5432 | ecommerce_user / SecurePassword123! |

## 🛠️ Configurar pgAdmin para Acessar PostgreSQL

1. Acesse http://localhost:5050
2. Login com: `admin@ecommerce.com` / `admin123`
3. Clique em "Add New Server"
4. Preencha:
   - **Name**: PostgreSQL - ECommerce
   - **Host name/address**: postgres-ecommerce
   - **Port**: 5432
   - **Username**: ecommerce_user
   - **Password**: SecurePassword123!
5. Clique em "Save"

## 🗄️ Gerenciar Banco de Dados

### Ver status do container PostgreSQL
```bash
docker-compose logs postgres-ecommerce
```

### Executar comandos no PostgreSQL
```bash
# Acessar psql
docker exec -it postgres-ecommerce psql -U ecommerce_user -d ECommerceDb

# Dentro do psql
\dt                    # Listar tabelas
\di                    # Listar índices
SELECT * FROM "Users"; # Query exemplo
\q                     # Sair
```

### Fazer backup do banco
```bash
docker exec postgres-ecommerce pg_dump -U ecommerce_user ECommerceDb > backup.sql
```

### Restaurar backup
```bash
docker exec -i postgres-ecommerce psql -U ecommerce_user ECommerceDb < backup.sql
```

## 📁 Estrutura de Volumes

```
postgres_data/          # Dados persistentes do PostgreSQL
  ├── base/
  ├── global/
  └── pg_wal/

logs/                   # Logs da aplicação
  ├── ecommerce-*.txt   # Logs diários
```

## 🔧 Variáveis de Ambiente

As variáveis estão configuradas em:
- `docker-compose.yml` - para containers
- `.env.docker` - referência de variáveis
- `appsettings.json` - configurações da aplicação

## ⚠️ Troubleshooting

### Erro: "postgres-ecommerce: Name or service not known"
```bash
# Verificar se os containers estão rodando
docker ps

# Verificar logs do container
docker-compose logs postgres-ecommerce
```

### Erro: "Connection refused"
```bash
# O PostgreSQL pode estar iniciando
# Aguarde alguns segundos e tente novamente
# Ou execute:
docker-compose restart ecommerce-api
```

### Erro: "Database already exists"
```bash
# Remover dados antigos
docker-compose down -v
docker-compose up -d
```

### Conexão no PostgreSQL recusada
```bash
# Verificar credenciais em docker-compose.yml
# Verificar se porta 5432 não está em uso
netstat -ano | findstr :5432
```

## 🔄 Workflows Úteis

### Recriar aplicação com novo build
```bash
docker-compose up -d --build ecommerce-api
```

### Recriar tudo do zero
```bash
docker-compose down -v
docker-compose up -d --build
```

### Ver uso de memória/CPU
```bash
docker stats
```

### Executar migrations manualmente
```bash
# Dentro do container da API
docker exec ecommerce-api dotnet ef database update \
  --project Infrastructure/Infrastructure.csproj
```

## 📚 Referências

- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [PostgreSQL Docker Image](https://hub.docker.com/_/postgres)
- [pgAdmin Docker Image](https://hub.docker.com/r/dpage/pgadmin4)
- [Entity Framework Core Migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)

---

**Desenvolvido com ❤️ usando .NET 10 + Docker + PostgreSQL**
