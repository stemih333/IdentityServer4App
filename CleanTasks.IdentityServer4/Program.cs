// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace CleanTasks.IdentityServer4
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var host = BuildWebHost(args);

            //var config = host.Services.GetRequiredService<IConfiguration>();
            //var connectionString = config.GetConnectionString("IdentityDbContext");
            //SeedData.EnsureSeedData(connectionString);
            //return;

            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args) => 
            WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .Build();
    }
}
