# KeyVault Lite - Security Documentation

## Security Model

### Encryption at Rest

All secrets are encrypted before being stored in the database using **AES-256-GCM**.

#### Encryption Process

1. **Key Derivation** (if needed):
   - Master key provided via `MASTER_KEY` environment variable
   - If master key is a passphrase, use PBKDF2 to derive 32-byte key
   - Salt: Application-specific constant

2. **Encryption**:
   - Generate random 12-byte IV for each secret
   - Encrypt secret value using AES-256-GCM
   - Store encrypted value + IV + authentication tag

3. **Storage**:
   - Encrypted value stored as `byte[]` in database
   - IV stored separately as `byte[]`
   - Authentication tag included in encrypted data

#### Decryption Process

1. Retrieve encrypted value, IV from database
2. Decrypt using master key and IV
3. Verify authentication tag
4. Return plaintext value

### Master Key Management

**Critical:** The master encryption key must be kept secure.

#### Recommended Practices

1. **Environment Variables:**
   ```bash
   export MASTER_KEY="your-32-byte-key-here"
   ```

2. **Docker Secrets:**
   ```yaml
   secrets:
     master_key:
       external: true
   ```

3. **Key Vault Integration:**
   - Azure Key Vault
   - AWS Secrets Manager
   - HashiCorp Vault

4. **Key Generation:**
   ```bash
   # Generate a secure 32-byte key
   openssl rand -hex 32
   ```

#### Key Rotation

Currently, key rotation requires:
1. Decrypt all secrets with old key
2. Re-encrypt with new key
3. Update master key

**Future Enhancement:** Implement key versioning to support seamless rotation.

### Transport Security

#### Development
- HTTP is acceptable for local development
- API runs on `http://localhost:5000`

#### Production
- **MUST** use HTTPS/TLS
- Configure reverse proxy (nginx, Traefik) with SSL certificates
- Use Let's Encrypt for free certificates

### Authentication & Authorization

#### Current State
- No authentication implemented (for simplicity)
- All endpoints are publicly accessible

#### Recommended Implementation

1. **JWT Authentication:**
   - User login endpoint
   - JWT token generation
   - Token validation middleware

2. **Role-Based Access Control (RBAC):**
   - Admin role: Full access
   - Reader role: Read-only access
   - Writer role: Create/Update access

3. **API Keys:**
   - Generate API keys for service-to-service communication
   - Store API keys securely
   - Validate API keys on each request

### Input Validation

All inputs are validated:

1. **Secret Name:**
   - Required
   - Max length: 255 characters
   - Alphanumeric, hyphens, underscores only
   - Unique constraint

2. **Secret Value:**
   - Required
   - Max length: 10,000 characters (configurable)

3. **Description:**
   - Optional
   - Max length: 1,000 characters

### SQL Injection Prevention

- Entity Framework Core uses parameterized queries
- No raw SQL queries with user input
- All database operations are type-safe

### XSS Prevention

- Web UI uses Vue 3's built-in XSS protection
- API returns JSON (not HTML)
- Content-Type headers properly set

### Secrets in Logs

**Critical:** Never log secret values.

- Only log secret metadata (name, ID, timestamps)
- Never log encrypted or decrypted values
- Use structured logging with sensitive data filtering

### Database Security

#### SQLite

- File permissions: `600` (owner read/write only)
- Store database file in secure location
- Regular backups with encryption

#### Production Databases

For production, use:
- PostgreSQL with encryption at rest
- SQL Server with Transparent Data Encryption (TDE)
- Connection strings stored securely

### Network Security

1. **Firewall Rules:**
   - Only expose necessary ports
   - Restrict access to internal networks

2. **CORS Configuration:**
   - Configure allowed origins
   - Restrict to known domains

3. **Rate Limiting:**
   - Implement rate limiting to prevent abuse
   - Use ASP.NET Core rate limiting middleware

### Audit Logging

**Recommended:** Implement audit logging for:
- Secret creation
- Secret retrieval
- Secret updates
- Secret deletion
- Failed authentication attempts

### Compliance Considerations

#### GDPR
- Right to deletion
- Data export capabilities
- Access logging

#### SOC 2
- Access controls
- Audit trails
- Encryption at rest and in transit

### Security Checklist

- [ ] Master key stored securely (not in code)
- [ ] HTTPS enabled in production
- [ ] Authentication implemented
- [ ] Input validation on all endpoints
- [ ] Secrets never logged
- [ ] Database file permissions secured
- [ ] CORS properly configured
- [ ] Rate limiting enabled
- [ ] Audit logging implemented
- [ ] Regular security updates
- [ ] Dependency vulnerability scanning

### Reporting Security Issues

If you discover a security vulnerability, please:
1. Do not open a public issue
2. Contact the maintainers privately
3. Provide detailed information about the vulnerability

