﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using IdentityServer.Data;
using IdentityServer.Models;
using IdentityServer.User;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Test;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace QuickstartIdentityServer
{
    public class Startup
    {
        private const string connString = @"
            
            ";
        
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
            var builder = new ConfigurationBuilder()
                .SetBasePath(hostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{hostingEnvironment.EnvironmentName}.json", optional: true)//增加环境配置文件，新建项目默认有
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IHostingEnvironment HostingEnvironment { get; }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddMvc();

            services.AddAuthenticationCore(options =>
            {
                options.AddScheme<MyAuthenticationHandler>("myScheme", "demo scheme");
            });


            services.AddTransient<IExtensionGrantValidator, MyCrapGrantValidator>();

            services.AddAuthentication()
                .AddMicrosoftAccount(MicrosoftAccountDefaults.AuthenticationScheme, options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.ClientId = "33f1f15d-93d5-4749-9b0e-24fc7c0bf56e";
                    options.ClientSecret = "wttGKYI05[vppzBAG913#?_";
                })
                .AddOpenIdConnect("Extend", "OA账号登录",options =>
                {
                    options.Authority = "http://127.0.0.1:7000";
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.SignOutScheme = IdentityServerConstants.SignoutScheme;
                    // Configure the Client ID and Client Secret
                    options.ClientId = "ZnwqE8j-H6kmHeQBM3NH2WbdikUjPrNV";
                    options.ClientSecret = "jecyL0PrTIxjNf4GUbz0oa_ssRLiJBG8OXfIMzLDjGCEoTV48HHqvK2pasPodPyN";
                    options.RequireHttpsMetadata = false;
                    // Set response type to code
                    options.ResponseType = "code";
                    options.Scope.Clear();
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.CallbackPath = new PathString("/callback");
                    options.GetClaimsFromUserInfoEndpoint = true;
                    // Configure the Claims Issuer
                    options.ClaimsIssuer = "Extend";
                    // Saves tokens to the AuthenticationProperties
                    options.SaveTokens = true;
                });
            // configure identity server with in-memory stores, keys, clients and scopes
            services.AddIdentityServer(options =>
                {
                    options.Authentication.CookieLifetime = TimeSpan.FromDays(1);
                    options.Authentication.CookieSlidingExpiration = true;
                })
                //.AddDeveloperSigningCredential()
                .AddSigningCredential(new X509Certificate2("",
                         Configuration["Certificates:Password"]))
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryClients(Config.GetClients())
                //.AddTestUsers(Config.GetUsers())
                
                //.AddConfigurationStore(options =>
                //{
                //    options.ConfigureDbContext = b =>
                //        b.UseSqlServer(connString,
                //            sql => sql.MigrationsAssembly(migrationAssembly));
                //})
                //.AddOperationalStore(options =>
                //{
                //    options.ConfigureDbContext = b =>
                //        b.UseSqlServer(connString,
                //            sql => sql.MigrationsAssembly(migrationAssembly));
                //})
                .AddExtensionGrantValidator<CzarCustomUserGrantValidator>()
                .AddProfileService<UserProfileService>();
            services.AddDbContext<CISDI_TEST20180829Context>(
                options => options.UseSqlServer(Configuration["ConnectionStrings"]));
            services.AddTransient<UserStore>();
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

            //InitializeDatabase(app);
            TestUserStore testUserStore = app.ApplicationServices.GetService<TestUserStore>();
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {

                var dbService = serviceScope.ServiceProvider.GetRequiredService<CISDI_TEST20180829Context>();
                //var dbService = app.ApplicationServices.GetService<CISDI_TEST20180829Context>();
                foreach (Popedom popedom in dbService.Popedom.Take(50))
                {
                    
                }
            }

            
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                if (!EnumerableExtensions.Any(context.Clients))
                {
                    foreach (var client in Config.GetClients())
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!EnumerableExtensions.Any(context.IdentityResources))
                {
                    foreach (var resource in Config.GetIdentityResources())
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!EnumerableExtensions.Any(context.ApiResources))
                {
                    foreach (var resource in Config.GetApiResources())
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}