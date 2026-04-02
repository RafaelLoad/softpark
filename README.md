# Softpark API - Gerenciamento de Usuários

API RESTful desenvolvida em .NET 8 com Arquitetura Limpa, autenticação JWT e acesso a dados com Dapper.

## Arquitetura

```
src/
├── Softpark.Api/               → Controllers (camada HTTP)
├── Softpark.Application/       → UseCases, DTOs e interfaces
├── Softpark.Domain/            → Entidades e regras de negócio
└── Softpark.Infrastructure/    → Repositories (Dapper) e serviços
```

**Fluxo de dependências:** Api → Application → Domain ← Infrastructure

## Pré-requisitos

- .NET 8 SDK ou superior
- SQL Server (banco `Entrevista` em `infra.softpark.com.br,14338`)

## Executando

```bash
cd src/Softpark.Api
dotnet run
```

A API estará disponível em `http://localhost:5000`. O Swagger UI pode ser acessado em `http://localhost:5000/swagger`.

---

## Endpoints

### Autenticação

#### `POST /api/auth/login`

Gera um token JWT. Este é o **único endpoint público**.

**Credenciais fixas:** `admin` / `123`

**Requisição:**

```json
{
  "usuario": "admin",
  "senha": "123"
}
```

**Resposta 200 OK:**

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**Resposta 401 Unauthorized:**

```json
{
  "mensagem": "Usuário ou senha inválidos."
}
```

---

### Usuários

> Todos os endpoints abaixo exigem o header `Authorization: Bearer {token}`.

#### `GET /api/usuarios`

Lista todos os usuários com paginação.

| Parâmetro      | Tipo  | Local | Padrão | Descrição                |
|----------------|-------|-------|--------|--------------------------|
| pagina         | int   | query | 1      | Número da página         |
| tamanhoPagina  | int   | query | 10     | Itens por página (max 100) |

**Resposta 200 OK:**

```json
{
  "items": [
    {
      "id": 1,
      "usuario": "joao.silva",
      "status": true,
      "perfis": ["Administrador", "Operador"]
    }
  ],
  "total": 1,
  "pagina": 1,
  "tamanhoPagina": 10
}
```

---

#### `GET /api/usuarios/{id}`

Obtém um usuário pelo ID.

| Parâmetro | Tipo | Local | Descrição     |
|-----------|------|-------|---------------|
| id        | int  | path  | ID do usuário |

**Resposta 200 OK:**

```json
{
  "id": 1,
  "usuario": "joao.silva",
  "status": true,
  "perfis": ["Administrador", "Operador"]
}
```

**Resposta 404 Not Found:**

```json
{
  "mensagem": "Usuário com ID 99 não encontrado."
}
```

---

#### `POST /api/usuarios`

Cria um novo usuário. Perfis são inseridos em transação atômica.

**Requisição:**

```json
{
  "usuario": "maria.souza",
  "status": true,
  "perfis": ["Operador"]
}
```

**Resposta 201 Created:**

```json
{
  "id": 2,
  "usuario": "maria.souza",
  "status": true,
  "perfis": ["Operador"]
}
```

**Resposta 400 Bad Request (validação):**

```json
{
  "mensagem": "O usuário deve ter pelo menos um perfil."
}
```

---

#### `PUT /api/usuarios/{id}`

Atualiza um usuário existente. Perfis são substituídos em transação atômica.

| Parâmetro | Tipo | Local | Descrição     |
|-----------|------|-------|---------------|
| id        | int  | path  | ID do usuário |

**Requisição:**

```json
{
  "usuario": "maria.souza.atualizado",
  "status": false,
  "perfis": ["Administrador", "Operador"]
}
```

**Resposta 200 OK:**

```json
{
  "id": 2,
  "usuario": "maria.souza.atualizado",
  "status": false,
  "perfis": ["Administrador", "Operador"]
}
```

**Resposta 404 Not Found:**

```json
{
  "mensagem": "Usuário com ID 99 não encontrado."
}
```

---

## Autenticação

Todos os endpoints, exceto `POST /api/auth/login`, exigem token JWT no header:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

Requisições sem token ou com token inválido retornam **401 Unauthorized**.

## Tecnologias

- **.NET 8** (C#)
- **Dapper** para acesso a dados
- **SQL Server** (tabelas `usuario` e `usuario_perfil`)
- **JWT** para autenticação
- **Serilog** para logging estruturado
- **Swagger/OpenAPI** para documentação interativa
