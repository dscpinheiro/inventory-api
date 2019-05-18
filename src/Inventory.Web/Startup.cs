using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using IdentityServer4.AccessTokenValidation;
using Inventory.Core.Data;
using Inventory.Core.Interfaces;
using Inventory.Core.Services;
using Inventory.Models;
using Inventory.Web.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Converters;
using Swashbuckle.AspNetCore.Swagger;

namespace Inventory.Web
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            services.AddDbContext<ApiDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDatabase");
                options.UseInternalServiceProvider(serviceProvider);
            });

            services.AddScoped<IShopService, ShopService>();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info { Title = "Inventory API", Version = "v1" });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);

                options.AddSecurityDefinition("oauth2", new OAuth2Scheme
                {
                    Flow = "implicit",
                    AuthorizationUrl = AuthConstants.AuthorizationUrl,
                    Scopes = new Dictionary<string, string> {{ AuthConstants.ApiScope, "Full access" }}
                });

                options.OperationFilter<AuthorizeOperationFilter>();
                options.DocumentFilter<RemoveModelsFilter>();
            });

            services
                .AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = AuthConstants.AuthorityUrl;
                    options.RequireHttpsMetadata = false;
                    options.ApiName = AuthConstants.ApiName;
                });

            services.AddCors();
            services
                .AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Inventory API V1");
                options.RoutePrefix = string.Empty;

                options.OAuthClientId("swagger_client");
                options.OAuthAppName("Inventory API - Swagger");
            });

            app.UseCors(b => b.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin());
            app.UseMvc();
        }
    }
}
