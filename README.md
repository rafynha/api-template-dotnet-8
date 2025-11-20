# Template WebAPI .NET 8

Template de API REST usando .NET 8 com Entity Framework Core e arquitetura em camadas.

## Estrutura do Projeto

```
src/
├── component.template.api/                 # API principal
├── component.template.business/            # Regras de negócio
├── component.template.configuration/       # Configurações
├── component.template.domain/              # Entidades e DTOs
├── component.template.infrastructure/      # Acesso a dados
└── component.template.business.test/       # Testes
```

## Configuração do Banco de Dados

O projeto suporta múltiplos bancos de dados configurados no `appsettings.json`:

- **SQL Server** (Ativo por padrão)
- **Cosmos DB** 
- **MongoDB**

### Connection String

```json
"Database": {
  "Sql": {
    "Active": true,
    "ConnectionString": "Server=(localdb)\\MSSQLLocalDB;Database=template;User Id=sa;Password=sa;TrustServerCertificate=True;"
  }
}
```

## Entity Framework Migrations

### Configuração Inicial

1. **Navegar para o projeto Infrastructure:**
   ```bash
   cd src/component.template.infrastructure
   ```

2. **Criar migration inicial:**
   ```bash
   dotnet ef migrations add InitialCreate --context SqlContext --startup-project ../component.template.api --output-dir Repository/SqlServer/Migrations
   ```

3. **Aplicar migration ao banco:**
   ```bash
   dotnet ef database update --context SqlContext --startup-project ../component.template.api
   ```

### Para Futuras Migrations

1. **Criar nova migration:**
   ```bash
   dotnet ef migrations add NomeDaMigration --context SqlContext --startup-project ../component.template.api --output-dir Repository/SqlServer/Migrations
   ```

2. **Aplicar ao banco:**
   ```bash
   dotnet ef database update --context SqlContext --startup-project ../component.template.api
   ```

### Comandos Úteis

```bash
# Listar migrations
dotnet ef migrations list --context SqlContext --startup-project ../component.template.api

# Reverter migration
dotnet ef database update NomeDaMigrationAnterior --context SqlContext --startup-project ../component.template.api

# Remover última migration (se não aplicada)
dotnet ef migrations remove --context SqlContext --startup-project ../component.template.api

# Gerar script SQL
dotnet ef migrations script --context SqlContext --startup-project ../component.template.api
```

## Estrutura de Migrations

As migrations ficam organizadas na seguinte estrutura:

```
component.template.infrastructure/
├── ExternalServices/
└── Repository/
    ├── Common/
    └── SqlServer/
        ├── UserRepository.cs
        └── Migrations/
            ├── 20231011000000_InitialCreate.cs
            └── SqlContextModelSnapshot.cs
```