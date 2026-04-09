📄 Documento de Modelagem de Domínio — E-commerce API (.NET)
🎯 Objetivo

Definir as entidades e relacionamentos de um sistema de e-commerce simplificado contendo:

Users (usuários)
Products (produtos)
Cart (carrinho)
Orders (pedidos)
Coupons (cupons de desconto)
🧱 Entidades do Domínio
👤 User
Descrição

Representa um usuário do sistema.

Propriedades
Id (Guid)
Name (string)
Email (string)
Relacionamentos
1:1 com Cart
1:N com Orders
🛒 Cart
Descrição

Carrinho ativo do usuário, onde os produtos são adicionados antes da compra.

Propriedades
Id (Guid)
UserId (Guid)
TotalAmount (decimal)
DiscountAmount (decimal)
FinalAmount (decimal)
CouponId (Guid, opcional)
Relacionamentos
N:1 com User
1:N com CartItems
0..1 com Coupon
📦 Product
Descrição

Representa um produto disponível no catálogo.

Propriedades
Id (Guid)
Name (string)
Description (string)
Price (decimal)
Stock (int)
Observação

O preço pode mudar ao longo do tempo.

🧾 CartItem
Descrição

Representa um item dentro do carrinho.

Propriedades
Id (Guid)
CartId (Guid)
ProductId (Guid)
Quantity (int)
Relacionamentos
N:1 com Cart
N:1 com Product
📑 Order
Descrição

Representa um pedido finalizado (histórico de compra).

Propriedades
Id (Guid)
UserId (Guid)
CreatedAt (DateTime)
TotalAmount (decimal)
DiscountAmount (decimal)
FinalAmount (decimal)
CouponCode (string)
Relacionamentos
N:1 com User
1:N com OrderItems
Observação

Os dados do pedido não devem ser alterados após a criação.

📄 OrderItem
Descrição

Representa um item dentro de um pedido.

Propriedades
Id (Guid)
OrderId (Guid)
ProductId (Guid)
Quantity (int)
UnitPrice (decimal)
Relacionamentos
N:1 com Order
N:1 com Product
Observação (CRÍTICA)
UnitPrice deve armazenar o valor do produto no momento da compra.
Isso garante consistência histórica, mesmo que o preço do produto mude depois.
🎟️ Coupon
Descrição

Representa um cupom de desconto.

Propriedades
Id (Guid)
Code (string)
DiscountPercentage (decimal)
ExpirationDate (DateTime)
IsActive (bool)
UsageLimit (int)
TimesUsed (int)
Relacionamentos
1:N com Cart (opcional)
🔗 Relacionamentos (Resumo)
User → Cart (1:1)
User → Orders (1:N)
Cart → CartItems (1:N)
CartItem → Product (N:1)
Order → OrderItems (1:N)
OrderItem → Product (N:1)
Cart → Coupon (0..1)
🔄 Fluxo de Negócio
Usuário é criado
Um carrinho é automaticamente associado ao usuário
Usuário adiciona produtos ao carrinho (CartItems)
Usuário aplica um cupom (opcional)
Sistema calcula:
TotalAmount
DiscountAmount
FinalAmount
Usuário finaliza a compra
O sistema:
Cria um Order
Converte CartItems em OrderItems
Salva o preço atual em UnitPrice
O carrinho pode ser limpo ou recriado
⚠️ Regras de Negócio Importantes
Order é imutável após criação
OrderItem deve armazenar o preço no momento da compra
Coupon deve ser validado:
ativo
não expirado
dentro do limite de uso
Cart pode ser alterado livremente até virar Order
TotalAmount, DiscountAmount e FinalAmount devem ser persistidos
💡 Observações Técnicas
Utilizar Entity Framework Core
Banco de dados: PostgreSQL
Usar Fluent API para relacionamentos complexos
Evitar lógica de negócio nas entidades (usar Services)
🚀 Sugestões de Extensão (futuro)
OrderStatus (Pending, Paid, Shipped)
Address (endereço de entrega)
Category (categoria de produto)
DiscountType (Percentage / Fixed)
Autenticação com JWT
🏁 Resumo

Este modelo separa claramente:

Dados mutáveis (Product, Cart)
Dados temporários (CartItem)
Dados imutáveis/históricos (Order, OrderItem)

Garantindo: