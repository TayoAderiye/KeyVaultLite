# KeyVault Lite - API Documentation

## Base URL

- Development: `http://localhost:5000`
- Production: `https://your-domain.com`

## Authentication

Currently, no authentication is required. In production, implement JWT or API key authentication.

## Endpoints

### Health Check

#### GET /health

Check if the API is running.

**Response:**
```json
{
  "status": "healthy",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

---

### List Secrets

#### GET /api/secrets

Retrieve all secrets (metadata only, not values).

**Query Parameters:**
- `tag` (optional): Filter by tag
- `search` (optional): Search in name and description

**Response:**
```json
{
  "secrets": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "name": "api-key",
      "description": "API key for external service",
      "tags": ["production", "api"],
      "createdAt": "2024-01-15T10:00:00Z",
      "updatedAt": "2024-01-15T10:00:00Z",
      "version": 1
    }
  ],
  "total": 1
}
```

**Status Codes:**
- `200 OK`: Success

---

### Get Secret

#### GET /api/secrets/{name}

Retrieve a specific secret by name (includes decrypted value).

**Path Parameters:**
- `name` (string, required): Secret name

**Response:**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "name": "api-key",
  "value": "actual-secret-value",
  "description": "API key for external service",
  "tags": ["production", "api"],
  "createdAt": "2024-01-15T10:00:00Z",
  "updatedAt": "2024-01-15T10:00:00Z",
  "version": 1
}
```

**Status Codes:**
- `200 OK`: Success
- `404 Not Found`: Secret not found

---

### Create Secret

#### POST /api/secrets

Create a new secret.

**Request Body:**
```json
{
  "name": "api-key",
  "value": "secret-value-to-encrypt",
  "description": "API key for external service",
  "tags": ["production", "api"]
}
```

**Fields:**
- `name` (string, required): Unique secret name (max 255 chars, alphanumeric, hyphens, underscores)
- `value` (string, required): Secret value to encrypt (max 10,000 chars)
- `description` (string, optional): Human-readable description (max 1,000 chars)
- `tags` (string[], optional): Array of tags for categorization

**Response:**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "name": "api-key",
  "description": "API key for external service",
  "tags": ["production", "api"],
  "createdAt": "2024-01-15T10:00:00Z",
  "updatedAt": "2024-01-15T10:00:00Z",
  "version": 1
}
```

**Status Codes:**
- `201 Created`: Secret created successfully
- `400 Bad Request`: Validation error
- `409 Conflict`: Secret with this name already exists

---

### Update Secret

#### PUT /api/secrets/{name}

Update an existing secret.

**Path Parameters:**
- `name` (string, required): Secret name

**Request Body:**
```json
{
  "value": "new-secret-value",
  "description": "Updated description",
  "tags": ["production", "api", "updated"]
}
```

**Fields:**
- `value` (string, optional): New secret value (will be re-encrypted)
- `description` (string, optional): Updated description
- `tags` (string[], optional): Updated tags

**Response:**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "name": "api-key",
  "description": "Updated description",
  "tags": ["production", "api", "updated"],
  "createdAt": "2024-01-15T10:00:00Z",
  "updatedAt": "2024-01-15T10:30:00Z",
  "version": 2
}
```

**Status Codes:**
- `200 OK`: Secret updated successfully
- `400 Bad Request`: Validation error
- `404 Not Found`: Secret not found

---

### Delete Secret

#### DELETE /api/secrets/{name}

Delete a secret permanently.

**Path Parameters:**
- `name` (string, required): Secret name

**Response:**
```json
{
  "message": "Secret deleted successfully"
}
```

**Status Codes:**
- `200 OK`: Secret deleted successfully
- `404 Not Found`: Secret not found

---

## Error Responses

All errors follow this format:

```json
{
  "error": {
    "code": "SECRET_NOT_FOUND",
    "message": "Secret with name 'api-key' not found",
    "details": {}
  }
}
```

### Common Error Codes

- `VALIDATION_ERROR`: Request validation failed
- `SECRET_NOT_FOUND`: Secret does not exist
- `SECRET_ALREADY_EXISTS`: Secret with this name already exists
- `ENCRYPTION_ERROR`: Error during encryption/decryption
- `DATABASE_ERROR`: Database operation failed
- `INTERNAL_ERROR`: Unexpected server error

## Rate Limiting

(To be implemented)
- Default: 100 requests per minute per IP
- Configurable via `appsettings.json`

## OpenAPI Specification

Full OpenAPI 3.0 specification available at:
- `/swagger` - Swagger UI
- `/swagger/v1/swagger.json` - OpenAPI JSON

## Examples

### cURL Examples

**List all secrets:**
```bash
curl http://localhost:5000/api/secrets
```

**Get a secret:**
```bash
curl http://localhost:5000/api/secrets/api-key
```

**Create a secret:**
```bash
curl -X POST http://localhost:5000/api/secrets \
  -H "Content-Type: application/json" \
  -d '{
    "name": "my-secret",
    "value": "secret-value",
    "description": "My secret",
    "tags": ["dev"]
  }'
```

**Update a secret:**
```bash
curl -X PUT http://localhost:5000/api/secrets/my-secret \
  -H "Content-Type: application/json" \
  -d '{
    "value": "new-secret-value"
  }'
```

**Delete a secret:**
```bash
curl -X DELETE http://localhost:5000/api/secrets/my-secret
```

### .NET SDK Example

See the `KeyVaultLite.Client` package documentation.

### JavaScript/TypeScript Example

```typescript
const response = await fetch('http://localhost:5000/api/secrets', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
  },
  body: JSON.stringify({
    name: 'my-secret',
    value: 'secret-value',
    description: 'My secret',
    tags: ['dev']
  })
});

const secret = await response.json();
```

