using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Inventory.Models;

namespace Inventory.IntegrationTests
{
    public static class AuthHelper
    {
        public static async Task<string> GetAccessToken()
        {
            var client = new HttpClient();
            var discovery = await client.GetDiscoveryDocumentAsync(AuthConstants.AuthorityUrl);

            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = discovery.TokenEndpoint,
                ClientId = "integrationtests_client",
                ClientSecret = "secret",
                Scope = AuthConstants.ApiScope
            });

            return tokenResponse.AccessToken;
        }
    }
}