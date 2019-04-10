// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using CleanTasks.Common.Constants;
using CleanTasks.CommonWeb.Classes;
using CleanTasks.IdentityServer4.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Reflection;

namespace CleanTasks.IdentityServer4
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("IdentityDbContext"),
                    opts => opts.MigrationsAssembly(assembly)));

            services.AddHttpContextAccessor();

            services.AddHttpClient("todoapi", async (c) =>
            {
                var serviceProvider = services.BuildServiceProvider();
                var httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
                var accessToken = await httpContextAccessor.HttpContext.GetTokenAsync("Cookie", "access_token");

                c.BaseAddress = new Uri(Configuration["apiUrl"]);
                c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            });

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddClaimsPrincipalFactory<CustomUserClaimsPrincipalFactory>();


            services.AddIdentityServer(opts => opts.Authentication.CookieLifetime = TimeSpan.FromHours(1))
                .AddDeveloperSigningCredential()
                .AddInMemoryIdentityResources(IdentityServerConfig.GetIdentityResources())
                .AddInMemoryApiResources(IdentityServerConfig.GetApiResources())
                .AddInMemoryClients(IdentityServerConfig.GetClients())
                .AddAspNetIdentity<ApplicationUser>();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;

                // User settings.
                options.User.RequireUniqueEmail = true;
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy", policy =>
                    policy.RequireClaim(AuthConstants.PermissionType, AuthConstants.UserAdminPermission));
                options.AddPolicy("UserPolicy", policy =>
                    policy.RequireClaim(AuthConstants.PermissionType, AuthConstants.UserPermission));
                options.AddPolicy("AllUserPolicy", policy =>
                    policy.RequireAssertion(
                        assert =>
                            assert.User.HasClaim(AuthConstants.PermissionType, AuthConstants.UserAdminPermission) ||
                            assert.User.HasClaim(AuthConstants.PermissionType, AuthConstants.UserPermission)));
            });


            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookie";
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie("Cookie", opts =>
            {
                opts.ExpireTimeSpan = TimeSpan.FromHours(1);
            })
            .AddOpenIdConnect("oidc", options =>
            {
                options.SignInScheme = "Cookie";
                options.Authority = "https://localhost:5000";
                options.ClientId = "adminGui_ID";
                options.ClientSecret = "AdminGUISecret";
                options.ResponseType = "code id_token";
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");
                options.Scope.Add("WebAPI");
                options.Scope.Add("offline_access");
                options.Scope.Add(AuthConstants.PermissionType);
                options.ClaimActions.Add(new JsonKeyArrayClaimAction(AuthConstants.PermissionType, AuthConstants.PermissionType, AuthConstants.PermissionType));
            });

            services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.Authority = "https://localhost:5000";
                options.RequireHttpsMetadata = true;
                options.Audience = "WebAPI";
            });


            // Customise default API behavour
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
                
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseIdentityServer();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseMvcWithDefaultRoute();
        }
    }
}