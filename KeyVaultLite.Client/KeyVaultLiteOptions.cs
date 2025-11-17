namespace KeyVaultLite.Client
{
    public sealed class KeyVaultLiteOptions
    {
        /// <summary>Base URL of the KeyVaultLite API, e.g. https://localhost:7283</summary>
        public string BaseUrl { get; set; } = string.Empty;

        /// <summary>Environment name to load secrets from, e.g. "Development", "Stagging", "Production".</summary>
        public string EnvironmentName { get; set; } = "Development";

        /// <summary>
        /// Optional project name to use as a prefix inside secrets, e.g. "Oracle".
        /// If set, the SDK will look for secrets starting with "Oracle/" and strip that prefix.
        /// </summary>
        public string? ProjectName { get; set; }

        /// <summary>Optional search term to filter secrets by name.</summary>
        public string? Search { get; set; }

        /// <summary>Optional secret-name prefixes to include. If set, only secrets whose names start with any of these will be loaded.</summary>
        public string[]? SecretNamePrefixes { get; set; }

        /// <summary>Use UserSecrets in development before pulling from KeyVaultLite.</summary>
        public bool UseUserSecretsInDevelopment { get; set; } = true;
    }
}
