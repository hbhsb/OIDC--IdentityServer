﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Claims;
using IdentityModel;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

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
                new IdentityResource("nationality","国籍", new List<string>(){"nationality"}),
                new IdentityResource("sysId","用户编号", new List<string>(){"sysId", "nationality"})
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("https://quickstarts/api",
                    "My API",
                    new List<string>()
                    {
                        "role",
                        "given_name",
                        "gender",
                        "nationality"
                    }),
                new ApiResource("java/api","JavaApi"),
                new ApiResource("api1",".Net Core Api")
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                //使用自定义认证方式的客户端
                new Client
                {
                    ClientId = "client",
                    AllowedGrantTypes = new List<string>()
                    {
                        "my_crap_grant",
                        "CzarCustomUser"
                    },

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OpenId,
                    }
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
                // 风险平台，OpenID Connect implicit flow client (MVC)
                new Client
                {
                    ClientId = "ZnwqE8j-H6kmHeQBM3NH2WbdikUjPrNV",
                    ClientName = "风险平台",
                    AllowedGrantTypes = GrantTypes.Code,
                    ClientSecrets =
                    {
                        new Secret("jecyL0PrTIxjNf4GUbz0oa_ssRLiJBG8OXfIMzLDjGCEoTV48HHqvK2pasPodPyN".Sha256())
                    },
                    //用于单点登出，当某一个应用发起远程登出请求后，identityServer4会调用这个地址完成此客户端的登出
                    FrontChannelLogoutSessionRequired = false,
                    FrontChannelLogoutUri = "http://192.168.0.174:3000/signout-remote",
                    //
                    RedirectUris = { "http://192.168.0.174:3000/callback", "http://localhost:3000/callback"},
                    PostLogoutRedirectUris = { "http://localhost:3000/" },
                    AllowOfflineAccess = true,
                    AllowedScopes =
                    {
                        "https://quickstarts/api",
                        "java/api",
                        "sysId",
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OpenId,
                    },
                    //AccessToken过期时长设置为5分钟
                    AccessTokenLifetime = 60*5,
                    //登录成功后跳过用户授权
                    RequireConsent = false,
                    AlwaysIncludeUserClaimsInIdToken = true
                },
                // 合同平台，OpenID Connect implicit flow client (MVC)
                new Client
                {
                    ClientId = "SYGtG8b4HcWBICHxkw63173ohARZoaO8",
                    ClientName = "合同平台",
                    AllowedGrantTypes = new List<string>{GrantType.AuthorizationCode, OpenIdConnectGrantTypes. RefreshToken },
                    ClientSecrets =
                    {
                        new Secret("BPptSgcAhYxUYIIPSbr5SDHG4-Gq8TrP2qsVc44j4YmNqmm-nuc2Ld3heyJQoMmB".Sha256())
                    },

                    RedirectUris = { "http://192.168.0.158:8196/callback" ,"http://localhost:3000/callback"},
                    PostLogoutRedirectUris = { "http://localhost:3000/" },
                    FrontChannelLogoutUri = "http://192.168.0.158:8196/account/logout",
                    FrontChannelLogoutSessionRequired = false,
                    AllowOfflineAccess = true,
                    AllowedScopes =
                    {
                        "https://quickstarts/api",
                        "java/api",
                        "api1",
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OpenId,
                        "nationality",
                        "sysId"
                    },
                    RequireConsent = false,
                    AccessTokenLifetime = 60,
                    AlwaysIncludeUserClaimsInIdToken = true
                },
                //new Client
                //{
                //    ClientId = "ThirdWebAppId",
                //    ClientName = "ThirdWebApp",
                //    AllowedGrantTypes = GrantTypes.Code,
                //    ClientSecrets =
                //    {
                //        new Secret("ThirdWebAppPsd".Sha256())
                //    },
                //    FrontChannelLogoutUri = "http://192.168.0.158:10112/account/logout",
                //    FrontChannelLogoutSessionRequired = false,
                //    RedirectUris = { "http://192.168.0.158:10112/callback" },
                //    PostLogoutRedirectUris = { "http://localhost:3000/" },

                //    AllowedScopes =
                //    {
                //        "https://quickstarts/api",
                //        "java/api",
                //        IdentityServerConstants.StandardScopes.Profile,
                //        IdentityServerConstants.StandardScopes.OpenId,
                //        "nationality"
                //    }
                //},
                new Client
                {
                    ClientId = "js",
                    ClientName = "JavaScript Client",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris =           { "http://localhost:3000/callback.html", "http://localhost:3000/silent.html"},
                    PostLogoutRedirectUris = { "http://localhost:3000/index.html" },
                    AllowedCorsOrigins =     { "http://localhost:3000" },
                    AccessTokenLifetime = 90,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "https://quickstarts/api",
                        "api1",
                        "sysId",
                    },
                    RequireConsent = false,
                    AllowOfflineAccess = true,
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
                        new Claim("website", "https://alice.com"),
                        new Claim("sysId","2222"),
                        new Claim("nationality","英国")
                    },
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
                        new Claim("sysId","1111"),
                        new Claim("nationality","美国")
                    }
                },
                new TestUser
                {
                    SubjectId = "hbhsb",
                    Username = "hbhsb",
                    Password = "hbhsb",

                    Claims = new List<Claim>
                    {
                        new Claim("iid","1"),
                        new Claim("pwd", "hbhsb"),
                        new Claim("website", "https://bob.com"),
                        new Claim("nationality","美国")
                    }
                }
            };
        }
    }
}