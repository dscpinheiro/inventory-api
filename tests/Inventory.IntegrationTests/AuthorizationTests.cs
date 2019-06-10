using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Inventory.Web;
using Inventory.Web.Requests;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace Inventory.IntegrationTests
{
    [Trait("Category", "Integration")]
    public class AuthorizationTests : IDisposable
    {
        private readonly TestServer _testServer;
        private readonly HttpClient _client;

        public AuthorizationTests()
        {
            _testServer = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _client = _testServer.CreateClient();
        }

        [Fact]
        public async Task Inventory_GetItemsWithoutAuth_ReturnsOk()
        {
            var response = await _client.GetAsync("inventory");

            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Purchases_BuyItemWithoutAuth_ReturnsUnauthorized()
        {
            var response = await _client.PostAsJsonAsync("purchases", new PurchaseRequest());

            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Purchases_BuyItemWithAuth_DoesNotFail()
        {
            var accessToken = await AuthHelper.GetAccessToken();
            _client.SetBearerToken(accessToken);

            var response = await _client.PostAsJsonAsync("purchases", new PurchaseRequest());
            Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        public void Dispose()
        {
            _client.Dispose();
            _testServer.Dispose();
        }
    }
}
