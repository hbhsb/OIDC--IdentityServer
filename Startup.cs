// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.IdentityModel.Tokens.Jwt;
using IdentityServer4;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace QuickstartIdentityServer
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddMvc();


            services.AddAuthentication()
                .AddMicrosoftAccount(MicrosoftAccountDefaults.AuthenticationScheme, options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.ClientId = "33f1f15d-93d5-4749-9b0e-24fc7c0bf56e";
                    options.ClientSecret = "wttGKYI05[vppzBAG913#?_";
                }).AddOpenIdConnect("Extend", options =>
                {
                    options.Authority = "http://192.168.0.158:10111";
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    // Configure the Auth0 Client ID and Client Secret
                    options.ClientId = "123";
                    options.ClientSecret = "456";
                    options.RequireHttpsMetadata = false;
                    // Set response type to code
                    options.ResponseType = "code";
                    options.Scope.Clear();
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.CallbackPath = new PathString("/callback");
                    options.GetClaimsFromUserInfoEndpoint = true;
                    // Configure the Claims Issuer to be Auth0
                    options.ClaimsIssuer = "Extend";

                    // Saves tokens to the AuthenticationProperties
                    options.SaveTokens = true;
                });
            // configure identity server with in-memory stores, keys, clients and scopes
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryClients(Config.GetClients())
                .AddTestUsers(Config.GetUsers());
            
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            app.UseIdentityServer();
            
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}