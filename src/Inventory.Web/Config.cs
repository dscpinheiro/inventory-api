using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Models;

namespace Inventory.Web
{
    public class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources = new List<IdentityResource>
        {
            new IdentityResources.OpenId()
        };

        public static IEnumerable<ApiResource> GetApis() => new List<ApiResource>
        {
            new ApiResource("inventoryapi", "Inventory API")
        };

        public static IEnumerable<Client> GetClients() => new List<Client>
        {
            new Client
            {
                ClientId = "authorized-client",

                // no interactive user, use the clientid/secret for authentication
                AllowedGrantTypes = GrantTypes.ClientCredentials,

                // secret for authentication
                ClientSecrets = { new Secret("secret".Sha256()) },

                // scopes that client has access to
                AllowedScopes = { "inventoryapi" }
            }
        };
    }
}