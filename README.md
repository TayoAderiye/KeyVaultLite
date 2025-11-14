# KeyVault Lite

A lightweight, self-hosted secret manager inspired by AWS Secrets Manager.

## Overview

KeyVault Lite provides a secure, self-hosted solution for managing secrets, API keys, and sensitive configuration data. It features:

- **RESTful API** - Full CRUD operations for secrets
- **Encryption at Rest** - Secrets are encrypted using AES-256-GCM
- **Web UI** - Vue 3 frontend for easy secret management
- **.NET SDK** - Official client library for .NET applications
- **Docker Support** - Easy deployment with Docker Compose

## Architecture

### Components

1. **Backend API** (.NET 8 Minimal API)
   - RESTful endpoints for secret management
   - AES-256-GCM encryption for secrets at rest
   - SQLite database for persistence
   - JWT authentication (optional)

2. **Web UI** (Vue 3)
   - Modern, responsive interface
   - Secret viewing and management
   - Real-time updates

3. **.NET SDK**
   - NuGet package for easy integration
   - Type-safe client library
   - Async/await support

### Security Model

- **Encryption**: All secrets are encrypted using AES-256-GCM before storage
- **Master Key**: A master encryption key is required (can be provided via environment variable)
- **Access Control**: Optional JWT-based authentication
- **No Plaintext Storage**: Secrets are never stored in plaintext

### Data Model

```
Secret
├── Id (Guid)
├── Name (string, unique)
├── Description (string, optional)
├── EncryptedValue (byte[])
├── EncryptionIV (byte[])
├── Tags (string[], optional)
├── CreatedAt (DateTime)
├── UpdatedAt (DateTime)
└── Version (int)
```

## Quick Start

### Prerequisites

- **.NET 8 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Node.js 20+** and npm
- **Docker and Docker Compose** (optional, for containerized deployment)

### Using Docker Compose (Recommended)

1. **Generate a master key:**
   ```bash
   export MASTER_KEY=$(openssl rand -hex 32)
   ```

2. **Start all services:**
   ```bash
   docker-compose up -d
   ```

3. **Access the application:**
   - API: `http://localhost:5000`
   - Web UI: `http://localhost:3000`
   - Swagger: `http://localhost:5000/swagger`

### Manual Setup

#### Backend

1. **Set the master key:**
   ```bash
   export MASTER_KEY=$(openssl rand -hex 32)
   ```

2. **Run the API:**
   ```bash
   cd KeyVaultLite.Api
   dotnet restore
   dotnet run
   ```

   The API will be available at `http://localhost:5000`

#### Frontend

1. **Install dependencies:**
   ```bash
   cd KeyVaultLite.Web
   npm install
   ```

2. **Run development server:**
   ```bash
   npm run dev
   ```

   The web app will be available at `http://localhost:3000`

## API Documentation

See [API.md](./docs/API.md) for detailed API documentation.

OpenAPI specification available at `/swagger` when running the API.

## SDK Usage

```csharp
using KeyVaultLite.Client;

var client = new KeyVaultClient("http://localhost:5000");

// Create a secret
var secret = await client.CreateSecretAsync(new CreateSecretRequest
{
    Name = "my-api-key",
    Value = "secret-value",
    Description = "API key for external service"
});

// Retrieve a secret
var retrieved = await client.GetSecretAsync("my-api-key");
Console.WriteLine($"Secret value: {retrieved.Value}");

// List all secrets
var secrets = await client.ListSecretsAsync();
```

## Project Structure

```
KeyVault_Lite/
├── KeyVaultLite.Api/          # .NET 8 Minimal API
├── KeyVaultLite.Client/       # .NET SDK
├── KeyVaultLite.Web/          # Vue 3 frontend
├── docs/                      # Documentation
│   ├── ARCHITECTURE.md
│   ├── SECURITY.md
│   ├── API.md
│   └── DEVELOPMENT.md
├── docker-compose.yml          # Docker Compose configuration
└── README.md
```

## Development

See [DEVELOPMENT.md](./docs/DEVELOPMENT.md) for development setup and guidelines.

## Security Considerations

⚠️ **Important**: 

- Never commit the master key to version control
- Use a strong, randomly generated master key (32 bytes)
- In production, use HTTPS/TLS
- Consider implementing authentication/authorization
- Regularly backup the database

See [SECURITY.md](./docs/SECURITY.md) for detailed security documentation.

## License

MIT
