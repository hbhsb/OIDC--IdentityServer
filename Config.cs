// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace QuickstartIdentityServer
{
    public class Config
    {
        // scopes define the resources in your system
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource("roles","角色", new List<string>{"role"}),
                new IdentityResource("nationality","国籍", new List<string>(){"nationality"})
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("https://quickstarts/api", "My API",
                    new List<string>()
                    {
                        "role",
                        "given_name",
                        "gender",
                        "nationality"
                    }),
                new ApiResource("java/api","JavaApi")
            };
        }

        // clients want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients()
        {
            // client credentials client
            return new List<Client>
            {
                new Client
                {
                    ClientId = "client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "api1" }
                },

                // resource owner password grant client
                new Client
                {
                    ClientId = "ro.client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "api1" }
                },
                // OpenID Connect implicit flow client (MVC)
                new Client
                {
                    ClientId = "ZnwqE8j-H6kmHeQBM3NH2WbdikUjPrNV",
                    ClientName = "MVC Client",
                    AllowedGrantTypes = GrantTypes.Code,
                    ClientSecrets =
                    {
                        new Secret("jecyL0PrTIxjNf4GUbz0oa_ssRLiJBG8OXfIMzLDjGCEoTV48HHqvK2pasPodPyN".Sha256())
                    },
                    FrontChannelLogoutUri = "http://192.168.0.174:3000/signout-remote",
                    FrontChannelLogoutSessionRequired = false,
                    RedirectUris = { "http://192.168.0.174:3000/callback" },
                    PostLogoutRedirectUris = { "http://localhost:3000/" },

                    AllowedScopes =
                    {
                        "https://quickstarts/api",
                        "java/api",
                        IdentityServerConstants.StandardScopes.OpenId,
                    }
                },
                new Client
                {
                    ClientId = "SYGtG8b4HcWBICHxkw63173ohARZoaO8",
                    ClientName = "MVC Client2",
                    AllowedGrantTypes = GrantTypes.Code,
                    ClientSecrets =
                    {
                        new Secret("BPptSgcAhYxUYIIPSbr5SDHG4-Gq8TrP2qsVc44j4YmNqmm-nuc2Ld3heyJQoMmB".Sha256())
                    },

                    RedirectUris = { "http://192.168.0.158:8196/callback" },
                    PostLogoutRedirectUris = { "http://localhost:3000/" },
                    FrontChannelLogoutUri = "http://192.168.0.158:8196/account/logout",
                    FrontChannelLogoutSessionRequired = false,
                    AllowedScopes =
                    {
                        "https://quickstarts/api",
                        "java/api",
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OpenId,
                        "nationality"
                    }
                },
                new Client
                {
                    ClientId = "ThirdWebAppId",
                    ClientName = "ThirdWebApp",
                    AllowedGrantTypes = GrantTypes.Code,
                    ClientSecrets =
                    {
                        new Secret("ThirdWebAppPsd".Sha256())
                    },
                    FrontChannelLogoutUri = "http://192.168.0.158:10112/account/logout",
                    FrontChannelLogoutSessionRequired = false,
                    RedirectUris = { "http://192.168.0.158:10112/callback" },
                    PostLogoutRedirectUris = { "http://localhost:3000/" },

                    AllowedScopes =
                    {
                        "https://quickstarts/api",
                        "java/api",
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OpenId,
                        "nationality"
                    }
                },
                new Client
                {
                    ClientId = "js",
                    ClientName = "JavaScript Client",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,

                    RedirectUris =           { "http://localhost:3000/callback.html" },
                    PostLogoutRedirectUris = { "http://localhost:3000/index.html" },
                    AllowedCorsOrigins =     { "http://localhost:3000" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "https://quickstarts/api"
                    }
                },
                new Client
                {
                    ClientId = "mvc2",
                    ClientName = "MVC Client2",
                    AllowedGrantTypes = GrantTypes.Hybrid,

                    RedirectUris = { "http://localhost:5004/signin-oidc" },
                    PostLogoutRedirectUris = { "http://localhost:5004/signout-callback-oidc" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    }
                }
            };
        }

        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "alice",
                    Password = "password",

                    Claims = new List<Claim>
                    {
                        new Claim("name", "Alice"),
                        new Claim("website", "https://alice.com")
                    }
                },
                new TestUser
                {
                    SubjectId = "3",
                    Username = "bob",
                    Password = "password",

                    Claims = new List<Claim>
                    {
                        new Claim("ID","123"),
                        new Claim("name", "Bob"),
                        new Claim("website", "https://bob.com"),
                        new Claim("综上所述","fuck"),
                        new Claim("nationality","美国")
                    }
                }
            };
        }
    }
}