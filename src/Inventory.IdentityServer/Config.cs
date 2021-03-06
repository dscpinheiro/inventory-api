﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Models;
using Inventory.Models;
using System.Collections.Generic;

namespace Inventory.IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources() => new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile()
        };

        public static IEnumerable<ApiResource> GetApis() => new List<ApiResource>
        {
            new ApiResource(AuthConstants.ApiName, "Inventory API")
        };

        public static IEnumerable<Client> GetClients() => new List<Client>
        {
            new Client
            {
                ClientId = "swagger_client",
                ClientName = "Swagger UI for Inventory API",
                AllowedGrantTypes = GrantTypes.Implicit,
                AllowAccessTokensViaBrowser = true,
                RedirectUris = { "http://localhost:5000/oauth2-redirect.html" },
                AllowedScopes = { AuthConstants.ApiScope }
            },
            new Client
            {
                ClientId = "integrationtests_client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("secret".Sha256()) },
                AllowedScopes = { AuthConstants.ApiScope }
            }
        };
    }
}