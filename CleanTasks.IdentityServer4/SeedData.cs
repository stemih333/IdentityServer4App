// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using CleanTasks.IdentityServer4.Identity;
using CleanTasks.Common.Constants;

namespace CleanTasks.IdentityServer4
{
    public class SeedData
    {
        public static void EnsureSeedData(string connectionString)
        {
            var services = new ServiceCollection();

            var assembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    connectionString,
                    opts => opts.MigrationsAssembly(assembly)));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddClaimsPrincipalFactory<CustomUserClaimsPrincipalFactory>();

            using (var serviceProvider = services.BuildServiceProvider())
            {
                using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                    context.Database.Migrate();

                    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                    var mihste = userMgr.FindByNameAsync("mihste").Result;
                    if (mihste == null)
                    {
                        mihste = new ApplicationUser
                        {
                            UserName = "mihste",
                            Email = "stemih11@gmail.com",
                            EmailConfirmed = true,
                            FirstName = "Stefan",
                            LastName = "Mihailovic",
                            CreatedBy = "SYSTEM",
                            UpdatedBy = "SYSTEM"
                        };
                        var result = userMgr.CreateAsync(mihste, "Stemih1!").Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }

                        result = userMgr.AddClaimsAsync(mihste, new Claim[]{
                        
                        new Claim(AuthConstants.PermissionType, AuthConstants.UserAdminPermission)
                    }).Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }
                        Console.WriteLine("alice created");
                    }
                    else
                    {
                        Console.WriteLine("alice already exists");
                    }


                    for(var i = 1; i <= 50; i++ )
                    {
                        var newUser = new ApplicationUser
                        {
                            UserName = "TestUsr" + i,
                            FirstName = "Test" + i,
                            LastName = "Usr" + i,
                            Email = $"testusr{i}@email.com",
                            EmailConfirmed = true,
                            CreatedBy = "SYSTEM",
                            UpdatedBy = "SYSTEM"
                        };

                        var result = userMgr.CreateAsync(newUser, "Stemih1!").Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }

                        result = userMgr.AddClaimsAsync(newUser, new Claim[]{
                            new Claim(AuthConstants.PermissionType, AuthConstants.UserPermission)
                        }).Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }
                    }
                }
            }
        }
    }
}
