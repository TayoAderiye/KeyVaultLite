# Development Guide

## Prerequisites

- .NET 8 SDK
- Node.js 20+ and npm
- Docker and Docker Compose (optional)

## Local Development

### Backend API

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

3. **Access Swagger UI:**
   Navigate to `http://localhost:5000/swagger`

### Frontend Web App

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

### Using Docker Compose

1. **Set the master key:**
   ```bash
   export MASTER_KEY=$(openssl rand -hex 32)
   ```

2. **Start all services:**
   ```bash
   docker-compose up -d
   ```

3. **View logs:**
   ```bash
   docker-compose logs -f
   ```

4. **Stop services:**
   ```bash
   docker-compose down
   ```

## Project Structure

```
KeyVault_Lite/
├── KeyVaultLite.Api/          # .NET 8 Minimal API
│   ├── Models/                # Data models and DTOs
│   ├── Services/              # Business logic
│   ├── Data/                  # Database context
│   └── Program.cs             # Application entry point
├── KeyVaultLite.Client/       # .NET SDK
│   ├── Models/                # Client models
│   └── KeyVaultClient.cs      # Main client class
├── KeyVaultLite.Web/          # Vue 3 frontend
│   ├── src/
│   │   ├── components/        # Vue components
│   │   ├── views/             # Page views
│   │   ├── services/          # API client
│   │   └── types/             # TypeScript types
│   └── package.json
└── docs/                      # Documentation
```

## Building

### Backend

```bash
cd KeyVaultLite.Api
dotnet build
dotnet publish -c Release
```

### Frontend

```bash
cd KeyVaultLite.Web
npm run build
```

### SDK

```bash
cd KeyVaultLite.Client
dotnet pack -c Release
```

## Testing

### API Testing

Use Swagger UI at `http://localhost:5000/swagger` or use curl:

```bash
# Create a secret
curl -X POST http://localhost:5000/api/secrets \
  -H "Content-Type: application/json" \
  -d '{
    "name": "test-secret",
    "value": "my-secret-value",
    "description": "Test secret"
  }'

# Get a secret
curl http://localhost:5000/api/secrets/test-secret

# List secrets
curl http://localhost:5000/api/secrets
```

## Database

The API uses SQLite by default. The database file is created at `keyvault.db` in the API directory.

To reset the database:
```bash
rm KeyVaultLite.Api/keyvault.db
```

## Troubleshooting

### Master Key Issues

If you see "MASTER_KEY environment variable is required":
- Set the `MASTER_KEY` environment variable
- For Docker, ensure it's set in `docker-compose.yml` or `.env` file

### Port Conflicts

If ports 5000 or 3000 are in use:
- Change ports in `docker-compose.yml`
- For API: Update `ASPNETCORE_URLS` environment variable
- For Web: Update `vite.config.ts` server port

### CORS Issues

If you see CORS errors:
- Ensure the API CORS policy allows your frontend origin
- Check `Program.cs` CORS configuration

## Contributing

1. Create a feature branch
2. Make your changes
3. Test thoroughly
4. Submit a pull request

## Code Style

- **C#**: Follow Microsoft C# coding conventions
- **TypeScript/Vue**: Follow Vue.js style guide
- Use meaningful variable names
- Add comments for complex logic
- Keep functions small and focused

