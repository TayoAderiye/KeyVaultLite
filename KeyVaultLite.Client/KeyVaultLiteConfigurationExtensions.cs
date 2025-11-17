using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace KeyVaultLite.Client
{
    public static class KeyVaultLiteConfigurationExtensions
    {
        public static async Task<WebApplicationBuilder> AddKeyVaultLiteAsync<TProgram>(
            this WebApplicationBuilder builder,
            Action<KeyVaultLiteOptions> configure,
            CancellationToken ct = default)
            where TProgram : class
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(configure);

            var options = new KeyVaultLiteOptions();
            configure(options);

            if (string.IsNullOrWhiteSpace(options.BaseUrl))
                throw new InvalidOperationException("KeyVaultLite BaseUrl is required.");

            // In dev, optionally load user secrets as well (like your AWS example)
            //if (builder.Environment.IsDevelopment() && options.UseUserSecretsInDevelopment)
            //{
            //    builder.Configuration.AddUserSecrets<TProgram>();
            //}

            // Derive default prefix from ProjectName if no explicit prefixes were provided
            string[]? prefixes = options.SecretNamePrefixes;
            if ((prefixes == null || prefixes.Length == 0) && !string.IsNullOrWhiteSpace(options.ProjectName))
            {
                prefixes = new[] { options.ProjectName!.TrimEnd('/') + "/" }; // e.g. "Oracle/"
            }

            // 1) Connect to KeyVaultLite
            using var client = new KeyVaultClient(options.BaseUrl);

            // 2) Resolve environment by name
            var env = await client.GetEnvironmentByNameAsync(options.EnvironmentName, ct) ?? throw new InvalidOperationException($"KeyVaultLite environment '{options.EnvironmentName}' was not found.");

            // 3) List secrets for that environment
            var list = await client.ListSecretsAsync(env.Id, options.Search, ct);
            var secretsToAdd = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // 4) For each secret, reveal its value and flatten into config keys
            foreach (var summary in list.Secrets)
            {
                string nameToUse = summary.Name;

                // If prefixes are defined, filter + strip them
                if (prefixes is { Length: > 0 })
                {
                    var matched = prefixes.FirstOrDefault(p => nameToUse.StartsWith(p, StringComparison.Ordinal));
                    if (matched is null)
                        continue; // not for this project

                    nameToUse = nameToUse.Substring(matched.Length); // strip "Oracle/" → "ApiKey"
                }

                // Nothing left? skip
                if (string.IsNullOrWhiteSpace(nameToUse))
                    continue;

                var rootSection = nameToUse.Replace("__", ":"); // Db__ConnectionString → Db:ConnectionString

                // Reveal secret value
                var secret = await client.RevealSecretAsync(env.Id, summary.Id, ct);
                if (secret?.Value is null)
                    continue;

                var rawValue = secret.Value;
                Dictionary<string, string> pairs =
                    LooksLikeJson(rawValue)
                        ? FlattenJson(rawValue)
                        : ParseKeyValuePairs(rawValue);

                if (pairs.Count == 0)
                {
                    // Whole string is one scalar config value
                    secretsToAdd[rootSection] = rawValue;
                }
                else
                {
                    foreach (var kv in pairs)
                    {
                        var subKey = kv.Key.Replace("__", ":");
                        var fullKey = string.IsNullOrEmpty(subKey)
                            ? rootSection
                            : $"{rootSection}:{subKey}";

                        secretsToAdd[fullKey] = kv.Value;
                    }
                }
            }

            builder.Configuration.AddInMemoryCollection(secretsToAdd!);
            return builder;

        }

        // Helpers copied/adapted from your AWS SecretManagerExtension

        private static bool LooksLikeJson(string s)
            => !string.IsNullOrWhiteSpace(s)
               && (s.TrimStart().StartsWith("{") || s.TrimStart().StartsWith("["));

        private static Dictionary<string, string> FlattenJson(string json)
        {
            using var doc = JsonDocument.Parse(json);
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            void Walk(JsonElement el, string prefix)
            {
                switch (el.ValueKind)
                {
                    case JsonValueKind.Object:
                        foreach (var p in el.EnumerateObject())
                            Walk(p.Value, string.IsNullOrEmpty(prefix) ? p.Name : $"{prefix}:{p.Name}");
                        break;

                    case JsonValueKind.Array:
                        int i = 0;
                        foreach (var item in el.EnumerateArray())
                            Walk(item, $"{prefix}:{i++}");
                        break;

                    default:
                        result[prefix] = el.ToString();
                        break;
                }
            }

            Walk(doc.RootElement, "");
            return result;
        }

        private static Dictionary<string, string> ParseKeyValuePairs(string text)
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // try newline-separated key=value first
            var lines = text.Replace("\r", "").Split('\n', StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length > 1)
            {
                foreach (var line in lines)
                {
                    var idx = line.IndexOf('=');
                    if (idx <= 0) continue;

                    var key = line[..idx].Trim();
                    var value = line[(idx + 1)..].Trim();

                    if (!string.IsNullOrEmpty(key))
                        dict[key] = value;
                }

                if (dict.Count > 0)
                    return dict;
            }

            return dict;
        }
    }
}

//var builder = WebApplication.CreateBuilder(args);

//builder = await builder.AddKeyVaultLiteAsync<Program>(options =>
//{
//    options.BaseUrl = "https://localhost:7283";
//    options.EnvironmentName = builder.Environment.EnvironmentName;
//    options.ProjectName = "MyService";
//});
//// now secrets are in builder.Configuration["MyApp:Db:ConnectionString"], etc.

//var conn = builder.Configuration["Db:ConnectionString"];
//var apiKey = builder.Configuration["ApiKey"];

//var app = builder.Build();
