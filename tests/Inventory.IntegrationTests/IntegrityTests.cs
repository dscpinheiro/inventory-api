using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Inventory.Models;
using Inventory.Web;
using Inventory.Web.Requests;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace Inventory.IntegrationTests
{
    [Trait("Category", "Integration")]
    public class IntegrityTests : IDisposable
    {
        private readonly TestServer _testServer;
        private readonly HttpClient _client;

        public IntegrityTests()
        {
            _testServer = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _client = _testServer.CreateClient();
        }

        [Fact]
        public async Task Purchases_AfterBuyingItem_AvailableUnitsAreUpdated()
        {
            var accessToken = await AuthHelper.GetAccessToken();
            _client.SetBearerToken(accessToken);

            var getItem = await _client.GetAsync($"inventory/64876c5f-0fc1-4a48-974e-da66d9c05630");
            var item = await getItem.Content.ReadAsAsync<Item>();
            var currentUnits = item.AvailableUnits;

            var buyItem = await _client.PostAsJsonAsync("purchases", new PurchaseRequest
            {
                ItemId = Guid.Parse("64876c5f-0fc1-4a48-974e-da66d9c05630"),
                Quantity = 10
            });

            Assert.Equal(HttpStatusCode.OK, buyItem.StatusCode);

            getItem = await _client.GetAsync($"inventory/64876c5f-0fc1-4a48-974e-da66d9c05630");
            item = await getItem.Content.ReadAsAsync<Item>();
            var newUnits = item.AvailableUnits;

            Assert.Equal(currentUnits - 10, newUnits);
        }

        public void Dispose()
        {
            _client.Dispose();
            _testServer.Dispose();
        }
    }
}