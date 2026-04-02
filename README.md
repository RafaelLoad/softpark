# Softpark API

API de gerenciamento de usuários em .NET 8 com Clean Architecture, Dapper e autenticação JWT.

## Estrutura do projeto

```
src/
├── Softpark.Api              Controllers e configurações HTTP
├── Softpark.Application      Services, DTOs e interfaces
├── Softpark.Domain           Entidades, enums e validações
└── Softpark.Infrastructure   Repositories (Dapper), autenticação e acesso a dados
```

Dependências: Api → Application → Domain ← Infrastructure

## Como rodar

Pré-requisitos: .NET 8 SDK

```bash
cd src/Softpark.Api
dotnet run --launch-profile Sqlite
```

Isso sobe a API na porta 5000 usando SQLite em memória (com dados de seed).

Para usar o SQL Server, rode com o profile `Development` ou `Production`.

### Swagger

Acesse `http://localhost:5000/swagger/index.html` para testar os endpoints direto pelo navegador.

No Swagger, clique em **Authorize**, faça login primeiro no endpoint `/api/auth/login` e cole o token no formato `Bearer {token}`.

## Profiles disponíveis

| Profile | Porta | Banco | Descrição |
|---------|-------|-------|-----------|
| Sqlite | 5000 | SQLite em memória | Desenvolvimento/testes sem banco externo |
| Development | 5000 | SQL Server | Desenvolvimento com banco real |
| Production | 5000 | SQL Server | Produção com logs reduzidos |

## Endpoints

### Login

`POST /api/auth/login` — único endpoint público

Credenciais fixas: `admin` / `123` (Temporariamente)

```json
// request
{ "usuario": "admin", "senha": "123" }

// response 200
{ "token": "eyJhbGciOiJIUzI1NiIs..." }

// response 401
{ "mensagem": "Usuário ou senha inválidos." }
```

Authorized "Bearer {token}"

### Listar usuários

`GET /api/usuarios?pagina=1&tamanhoPagina=10`

Paginação no SQL com LIMIT/OFFSET. `tamanhoPagina` aceita no máximo 100.

```json
// response 200
{
  "items": [
    {
      "id": 1,
      "usuario": "joao.silva",
      "status": true,
      "perfis": ["Administrador", "Gerente"]
    }
  ],
  "total": 3,
  "pagina": 1,
  "tamanhoPagina": 10
}
```

### Obter por ID

`GET /api/usuarios/{id}`

```json
// response 200
{
  "id": 1,
  "usuario": "joao.silva",
  "status": true,
  "perfis": ["Administrador", "Gerente"]
}

// response 404
{ "mensagem": "Usuário com ID 99 não encontrado." }
```

### Criar usuário

`POST /api/usuarios`

O nome do usuário não pode estar duplicado. Perfis são validados contra o enum (`Administrador`, `Gerente`, `Usuario`, `Consultor`). Insert feito com transação atômica.

```json
// request
{
  "usuario": "maria.souza",
  "status": true,
  "perfis": ["Usuario"]
}

// response 201
{
  "id": 4,
  "usuario": "maria.souza",
  "status": true,
  "perfis": ["Usuario"]
}

// response 400 (nome duplicado)
{ "mensagem": "Já existe um usuário com o nome 'maria.souza'." }

// response 400 (perfil inválido)
{ "mensagem": "Perfil(is) inválido(s): Diretor. Valores permitidos: Administrador, Gerente, Usuario, Consultor." }
```

### Atualizar usuário

`PUT /api/usuarios/{id}`

Os perfis antigos são removidos e os novos inseridos em transação atômica.

```json
// request
{
  "usuario": "maria.souza.editado",
  "status": false,
  "perfis": ["Administrador", "Consultor"]
}

// response 200
{
  "id": 4,
  "usuario": "maria.souza.editado",
  "status": false,
  "perfis": ["Administrador", "Consultor"]
}

// response 404
{ "mensagem": "Usuário com ID 99 não encontrado." }
```

## Autenticação

Todos os endpoints (exceto login) exigem o header:

```
Authorization: Bearer {token}
```

Sem token ou com token inválido/expirado: **401 Unauthorized**.

## Stack

- .NET 8 / C#
- Dapper (sem Entity Framework)
- SQL Server / SQLite
- JWT (Bearer token)
- Serilog
- Swagger (Swashbuckle)
