namespace KeyVaultLite.Api.Helpers
{
    public static class RouteHelper
    {
        public const string Version1 = "api/v1/vault/";

        public static class EnvironmentRoutes
        {
            public const string Base = "environments";
            public const string ById = Base + "/{id:guid}";
            public const string ByName = Base + "/{name}";
            public const string Create = Base + "/create";
            public const string SecretsByEnvId = Base + "/{id:guid}/secrets";
            public const string SecretByEnvId = Base + "/{id:guid}/secrets/{name}";
        }
        public static class EncryptionKeyRoutes
        {
            public const string Base = "keys";
            public const string ById = Base + "/{id:guid}";
            public const string Create = Base + "/create";
        }
        public static class SecretsRoutes
        {
            public const string Base = "secrets";
            public const string ById = Base + "/{envId:guid}/{secretId:guid}";
            public const string Reveal = Base + "/{envId:guid}/reveal/{secretId:guid}";
            public const string Create = Base + "/create";
        }
    }
}
