# 🚀 CryptoWatcher

Sistema de monitoramento de preços de criptomoedas com notificações em tempo real, desenvolvido com arquitetura limpa e práticas enterprise-grade.

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat&logo=dotnet)
![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?style=flat&logo=docker)
![RabbitMQ](https://img.shields.io/badge/RabbitMQ-Messaging-FF6600?style=flat&logo=rabbitmq)
![Redis](https://img.shields.io/badge/Redis-Cache-DC382D?style=flat&logo=redis)

## 📋 Sobre o Projeto

CryptoWatcher permite que usuários criem alertas personalizados para serem notificados quando o preço de uma criptomoeda atinge um valor específico. O sistema monitora preços em tempo real através da API do CoinGecko e processa notificações de forma assíncrona usando filas de mensagens.

### ✨ Funcionalidades

- **Alertas Personalizados**: Configure alertas para serem disparados quando o preço estiver acima ou abaixo de um valor específico
- **Monitoramento Contínuo**: Worker service que verifica preços a cada minuto
- **Notificações Assíncronas**: Sistema de filas com RabbitMQ para processamento desacoplado
- **Cache Inteligente**: Redis para reduzir chamadas à API externa e melhorar performance
- **Resiliência**: Retry policies e circuit breakers para maior confiabilidade
- **API REST**: Interface completa com documentação Swagger

## 🏗️ Arquitetura

### Clean Architecture
```
┌─────────────────────────────────────────────────────┐
│                    Presentation                      │
│              (API Controllers, Worker)               │
├─────────────────────────────────────────────────────┤
│                    Application                       │
│          (Use Cases, DTOs, Interfaces)              │
├─────────────────────────────────────────────────────┤
│                   Infrastructure                     │
│    (EF Core, Redis, RabbitMQ, External APIs)        │
├─────────────────────────────────────────────────────┤
│                      Domain                          │
│           (Entities, Business Rules)                 │
└─────────────────────────────────────────────────────┘
```

### Fluxo de Dados
```
┌──────────┐    ┌──────────┐    ┌──────────┐    ┌──────────┐
│   User   │───▶│   API    │───▶│ Database │    │ CoinGecko│
│          │    │          │    │ (SQL)    │    │   API    │
└──────────┘    └──────────┘    └──────────┘    └────┬─────┘
                                                      │
┌──────────────────────────────────────────────────────┼─────┐
│                    Worker Service                    │     │
│  ┌────────────┐    ┌─────────┐    ┌──────────────┐ │     │
│  │  Monitor   │───▶│  Redis  │───▶│   RabbitMQ   │◀┘     │
│  │  Prices    │    │ (Cache) │    │   (Queue)    │       │
│  └────────────┘    └─────────┘    └──────┬───────┘       │
│                                           │               │
│                    ┌──────────────────────▼────┐          │
│                    │  Notification Consumer    │          │
│                    │   (Email/SMS/Webhook)     │          │
│                    └───────────────────────────┘          │
└───────────────────────────────────────────────────────────┘
```

## 🛠️ Tecnologias Utilizadas

### Backend
- **.NET 9.0** - Framework principal
- **Entity Framework Core 9.0** - ORM para acesso a dados
- **ASP.NET Core** - Web API
- **Worker Services** - Background tasks

### Infraestrutura
- **SQL Server 2022** - Banco de dados relacional
- **Redis 7** - Cache em memória
- **RabbitMQ 3** - Message broker
- **Docker & Docker Compose** - Containerização

### Bibliotecas
- **MassTransit 8.x** - Abstração para mensageria
- **Polly** - Resiliência (retry, circuit breaker)
- **StackExchange.Redis** - Cliente Redis
- **Swashbuckle (Swagger)** - Documentação da API

### Padrões e Práticas
- Clean Architecture
- SOLID Principles
- Repository Pattern
- Decorator Pattern (Cached Services)
- Domain-Driven Design (DDD)
- Dependency Injection

## 🚀 Como Executar

### Pré-requisitos
- [Docker](https://www.docker.com/get-started) e Docker Compose
- (Opcional) [.NET 9 SDK](https://dotnet.microsoft.com/download) para desenvolvimento local

### Executar com Docker (Recomendado)

1. **Clone o repositório**
```bash
git clone https://github.com/seu-usuario/CryptoWatcher.git
cd CryptoWatcher
```

2. **Suba toda a infraestrutura**
```bash
docker-compose up -d
```

3. **Aguarde os serviços iniciarem** (30-60 segundos)
```bash
docker-compose logs -f api worker
```

4. **Acesse a API**
- Swagger UI: http://localhost:5000/swagger
- RabbitMQ Management: http://localhost:15672 (admin/admin123)

### Executar Localmente (Desenvolvimento)

1. **Suba apenas a infraestrutura**
```bash
docker-compose up -d sqlserver redis rabbitmq
```

2. **Configure as connection strings**
```bash
# Em appsettings.Development.json
# Já configurado para localhost
```

3. **Execute as migrations**
```bash
dotnet ef database update --project CryptoWatcher.Infrastructure --startup-project CryptoWatcher.API
```

4. **Rode a API e o Worker**
```bash
# Terminal 1
cd CryptoWatcher.API
dotnet run

# Terminal 2
cd CryptoWatcher.Worker
dotnet run
```

## 📚 Uso da API

### Criar um Usuário
```bash
POST /api/users
Content-Type: application/json

{
  "email": "user@example.com",
  "name": "João Silva"
}
```

### Criar um Alerta
```bash
POST /api/alerts
Content-Type: application/json

{
  "userId": 1,
  "cryptoSymbol": "BTC",
  "targetPrice": 50000,
  "condition": 2
}
```

**Condições:**
- `1` = Above (acima do preço alvo)
- `2` = Below (abaixo do preço alvo)

### Listar Alertas Ativos
```bash
GET /api/alerts/active
```

## 📁 Estrutura do Projeto
```
CryptoWatcher/
├── CryptoWatcher.Domain/              # Entidades e regras de negócio
│   ├── Entities/
│   ├── Enums/
│   └── Common/
├── CryptoWatcher.Application/         # Casos de uso e interfaces
│   ├── DTOs/
│   ├── Interfaces/
│   └── UseCases/
├── CryptoWatcher.Infrastructure/      # Implementações técnicas
│   ├── Data/                          # DbContext e Configurations
│   ├── Repositories/
│   └── Services/
├── CryptoWatcher.API/                 # API REST
│   ├── Controllers/
│   └── Dockerfile
├── CryptoWatcher.Worker/              # Background Service
│   ├── Workers/
│   ├── Services/
│   ├── Consumers/
│   └── Dockerfile
└── docker-compose.yml                 # Orquestração de containers
```

## 🧪 Testes
```bash
# Executar testes (quando implementados)
dotnet test
```

## 🔒 Segurança

- Senhas e connection strings sensíveis devem ser gerenciadas via **User Secrets** (desenvolvimento) ou **Environment Variables** (produção)
- O `appsettings.Development.json` está no `.gitignore`
- Em produção, use **Azure Key Vault** ou similar

## 📈 Roadmap

- [ ] Implementar envio real de emails (SendGrid/SMTP)
- [ ] Adicionar autenticação JWT
- [ ] Dashboard web com React/Blazor
- [ ] Testes unitários e de integração
- [ ] CI/CD com GitHub Actions
- [ ] Deploy em Azure/AWS
- [ ] Suporte a múltiplas exchanges
- [ ] Webhooks customizáveis
