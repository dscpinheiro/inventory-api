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
    public class ValidationTests : IDisposable
    {
        private readonly TestServer _testServer;
        private readonly HttpClient _client;

        public ValidationTests()
        {
            _testServer = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _client = _testServer.CreateClient();
        }

        [Theory, ClassData(typeof(InvalidRequestData))]
        public async Task Purchases_InvalidRequest_ReturnsBadRequest(Guid? itemId, int? quantity)
        {
            var accessToken = await AuthHelper.GetAccessToken();
            _client.SetBearerToken(accessToken);

            var response = await _client.PostAsJsonAsync("purchases", new PurchaseRequest
            {
                ItemId = itemId,
                Quantity = quantity
            });

            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        public void Dispose()
        {
            _client.Dispose();
            _testServer.Dispose();
        }
    }

    public class InvalidRequestData : TheoryData<Guid?, int?>
    {
        public InvalidRequestData()
        {
            // Empty item id
            Add(null, 1);

            // Empty quantity
            Add(Guid.NewGuid(), null);

            // Invalid quantities
            Add(Guid.NewGuid(), -1);
            Add(Guid.NewGuid(), 0);
            Add(Guid.NewGuid(), 51);
        }
    }
}