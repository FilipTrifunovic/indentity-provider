using Duende.IdentityServer.Models;
using IdentityModel;

namespace IdentityServer;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        [
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResource
            {
                Name = "email",
                UserClaims = { JwtClaimTypes.Email },
            }
        ];

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
            {
                new ApiScope(name: "product", displayName: "Get Products")
            };

    public static IEnumerable<ApiResource> Apis => new ApiResource[] {
         new ApiResource("api", "ElasticSearch API")
            {
                Scopes = { "product" }
            },
    };

    public static IEnumerable<Client> Clients =>
        [
             new Client
            {
                ClientId = "angular_spa",
                AllowedGrantTypes = GrantTypes.Code,
                RequireClientSecret = false,
                RedirectUris = { "http://localhost:4200/callback" },
                PostLogoutRedirectUris = { "http://localhost:4200" },
                AllowedCorsOrigins = { "http://localhost:4200" },
                AllowedScopes = { "openid", "profile","email","product" },
                RequirePkce = true,
                AllowAccessTokensViaBrowser = true,
                AllowOfflineAccess = true, 
                RefreshTokenUsage = TokenUsage.ReUse,
                RefreshTokenExpiration = TokenExpiration.Absolute,
                AbsoluteRefreshTokenLifetime = 2592000,
            }
        ];
}