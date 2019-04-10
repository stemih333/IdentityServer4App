using CleanTasks.Common.Constants;
using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace CleanTasks.IdentityServer4
{
    public static class IdentityServerConfig
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
            => new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource(AuthConstants.PermissionType, "Users permissions to application", new []{ AuthConstants.PermissionType }),
                new IdentityResource(PermissionTypes.TodoAreaPermission, "Todo area permissions", new []{ PermissionTypes.TodoAreaPermission })
            };

        public static IEnumerable<ApiResource> GetApiResources()
            => new List<ApiResource>
            {
                new ApiResource("WebAPI", "Web API")
                {
                    Description = "REST API with Todo action endpoints.",
                    Enabled = true
                }
            };

        public static IEnumerable<Client> GetClients()
            => new List<Client>
            {
                new Client
                {
                    ClientId = "razorgui_ID",
                    ClientName = "RazorGUI",
                    ClientSecrets = new List<Secret> { new Secret("RazorGUISecret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    //AlwaysIncludeUserClaimsInIdToken = false,
                    RedirectUris = new List<string> { "https://localhost:5002/signin-oidc" },
                    PostLogoutRedirectUris = new List<string> { "https://localhost:5002/signout-callback-oidc" },
                    BackChannelLogoutUri = "https://localhost:5002/",
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "WebAPI",
                        AuthConstants.PermissionType,
                        PermissionTypes.TodoAreaPermission
                    },
                    Description = "MVC Razor Pages GUI.",
                    AllowOfflineAccess = true,
                    UpdateAccessTokenClaimsOnRefresh = true,
                    IdentityTokenLifetime = 3600
                },
                new Client
                {
                    ClientId = "adminGui_ID",
                    ClientName = "AdminGUI",
                    ClientSecrets = new List<Secret> { new Secret("AdminGUISecret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    //AlwaysIncludeUserClaimsInIdToken = false,
                    RedirectUris = new List<string> { "https://localhost:5000/signin-oidc" },
                    PostLogoutRedirectUris = new List<string> { "https://localhost:5000/signout-callback-oidc" },
                    BackChannelLogoutUri = "https://localhost:5000/",
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "WebAPI",
                        AuthConstants.PermissionType,
                        PermissionTypes.TodoAreaPermission
                    },
                    Description = "MVC Admin GUI.",
                    AllowOfflineAccess = true,
                    UpdateAccessTokenClaimsOnRefresh = true,
                    IdentityTokenLifetime = 3600
                }
            };   
    }
}
