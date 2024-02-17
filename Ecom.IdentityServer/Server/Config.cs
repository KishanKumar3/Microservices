using IdentityServer4.Models;

namespace Server
{
    public class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource
                (
                    "roles",
                    "Your role(s)",
                    new List<string> {"role"}
                ) ,
                new IdentityResource
                (
                    "claims", 
                    "Your claims",
                    new List<string> {"user-id"}
                )
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new[] { new ApiScope("EComAPI.read"), new ApiScope("EComAPI.write"), };
        public static IEnumerable<ApiResource> ApiResources =>
            new[]
            {
                new ApiResource("EComAPI")
                {
                    Scopes = new List<string> { "EComAPI.read", "EComAPI.write" },
                    ApiSecrets = new List<Secret> { new Secret("ScopeSecret".Sha256()) },
                    UserClaims = new List<string> { "role", "user-id" }
                }
            };

        public static IEnumerable<Client> Clients =>
            new[]
            {
                // m2m client credentials flow client
                new Client
                {
                    ClientId = "m2m.client",
                    ClientName = "Client Credentials Client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("ClientSecret1".Sha256()) },
                    AllowedScopes = { "EComAPI.read", "EComAPI.write" }
                },

                new Client()
                {
                    ClientId = "roles_client",
                    ClientName = "Roles Test Client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "EComAPI.read", "roles", "claims" },
                    RequireClientSecret = false,
                    AlwaysIncludeUserClaimsInIdToken = true
                }

            };
    }
}
