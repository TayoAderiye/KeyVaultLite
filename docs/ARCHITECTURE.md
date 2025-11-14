# KeyVault Lite - Architecture Documentation

## System Architecture

### High-Level Overview

```
┌─────────────┐
│   Vue 3 UI  │
│  (Port 3000)│
└──────┬──────┘
       │ HTTP/REST
       │
┌──────▼──────────────────┐
│   .NET 8 API            │
│   (Port 5000)           │
│                         │
│  ┌──────────────────┐   │
│  │  Encryption      │   │
│  │  Service         │   │
│  └──────────────────┘   │
│                         │
│  ┌──────────────────┐   │
│  │  Secret Service  │   │
│  └──────────────────┘   │
│                         │
│  ┌──────────────────┐   │
│  │  SQLite DB       │   │
│  └──────────────────┘   │
└─────────────────────────┘
```

## Component Details

### 1. Backend API (KeyVaultLite.Api)

**Technology Stack:**
- .NET 8
- Minimal API
- Entity Framework Core (SQLite)
- ASP.NET Core

**Key Responsibilities:**
- RESTful API endpoints
- Request validation
- Encryption/decryption orchestration
- Database operations
- Error handling

**Project Structure:**
```
KeyVaultLite.Api/
├── Program.cs              # Application entry point
├── Models/
│   ├── Secret.cs          # Data model
│   └── DTOs/              # Request/Response DTOs
├── Services/
│   ├── IEncryptionService.cs
│   ├── EncryptionService.cs
│   ├── ISecretService.cs
│   └── SecretService.cs
├── Data/
│   └── KeyVaultDbContext.cs
└── appsettings.json
```

### 2. Encryption Service

**Algorithm:** AES-256-GCM

**Key Management:**
- Master key provided via environment variable `MASTER_KEY`
- Key derivation using PBKDF2 (if needed)
- Each secret uses a unique IV (Initialization Vector)

**Encryption Flow:**
1. Generate random IV (12 bytes for GCM)
2. Encrypt secret value using AES-256-GCM
3. Store encrypted value + IV in database
4. Return encrypted data

**Decryption Flow:**
1. Retrieve encrypted value + IV from database
2. Decrypt using master key and IV
3. Return plaintext value

### 3. Data Model

**Secret Entity:**
```csharp
public class Secret
{
    public Guid Id { get; set; }
    public string Name { get; set; }          // Unique identifier
    public string? Description { get; set; }
    public byte[] EncryptedValue { get; set; }
    public byte[] EncryptionIV { get; set; }
    public string? Tags { get; set; }         // JSON array
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int Version { get; set; }
}
```

**Database Schema:**
- SQLite database file: `keyvault.db`
- Single table: `Secrets`
- Indexes on `Name` for fast lookups

### 4. Web UI (KeyVaultLite.Web)

**Technology Stack:**
- Vue 3 (Composition API)
- TypeScript
- Vite
- Axios for HTTP requests
- Tailwind CSS (optional, for styling)

**Features:**
- List all secrets
- View secret details
- Create new secrets
- Update existing secrets
- Delete secrets
- Search/filter secrets

**Project Structure:**
```
KeyVaultLite.Web/
├── src/
│   ├── main.ts
│   ├── App.vue
│   ├── components/
│   │   ├── SecretList.vue
│   │   ├── SecretForm.vue
│   │   └── SecretView.vue
│   ├── services/
│   │   └── api.ts
│   └── types/
│       └── secret.ts
├── package.json
└── vite.config.ts
```

### 5. .NET SDK (KeyVaultLite.Client)

**Purpose:** Provide a type-safe, easy-to-use client library for .NET applications.

**Features:**
- Full async/await support
- Strongly-typed models
- Error handling
- HTTP client abstraction

**Project Structure:**
```
KeyVaultLite.Client/
├── KeyVaultClient.cs      # Main client class
├── Models/
│   ├── Secret.cs
│   ├── CreateSecretRequest.cs
│   └── UpdateSecretRequest.cs
└── KeyVaultLite.Client.csproj
```

## Security Considerations

### Encryption

- **Algorithm:** AES-256-GCM (Galois/Counter Mode)
- **Key Size:** 256 bits (32 bytes)
- **IV Size:** 12 bytes (96 bits) for GCM
- **Tag Size:** 16 bytes (128 bits) for authentication

### Key Management

- Master key must be provided at startup
- Key should be stored securely (environment variable, key vault, etc.)
- Never commit master key to version control

### Best Practices

1. **Transport Security:** Use HTTPS in production
2. **Key Rotation:** Implement key rotation strategy
3. **Access Control:** Add authentication/authorization
4. **Audit Logging:** Log all secret access
5. **Backup:** Regular database backups

## API Endpoints

### Secrets Management

- `GET /api/secrets` - List all secrets
- `GET /api/secrets/{name}` - Get secret by name
- `POST /api/secrets` - Create new secret
- `PUT /api/secrets/{name}` - Update existing secret
- `DELETE /api/secrets/{name}` - Delete secret

### Health & Info

- `GET /health` - Health check
- `GET /swagger` - OpenAPI documentation

## Deployment

### Docker

- Single container for API
- Single container for Web UI
- Volume mount for SQLite database
- Environment variables for configuration

### Production Considerations

1. Use PostgreSQL or SQL Server instead of SQLite
2. Implement proper authentication (JWT, OAuth2)
3. Enable HTTPS/TLS
4. Set up monitoring and logging
5. Implement rate limiting
6. Add request validation and sanitization

## Scalability

### Current Limitations

- SQLite is single-writer
- No distributed caching
- No load balancing support

### Future Enhancements

- Support for PostgreSQL/SQL Server
- Redis caching layer
- Horizontal scaling support
- Multi-region replication

