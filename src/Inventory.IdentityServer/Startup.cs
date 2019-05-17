// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using IdentityServer4;
using IdentityServer4.Quickstart.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Inventory.IdentityServer
{
    public class Startup
    {
        public IHostingEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddIdentityServer()
                .AddTestUsers(TestUsers.Users)
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApis())
                .AddInMemoryClients(Config.GetClients())
                .AddDeveloperSigningCredential(false);
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}