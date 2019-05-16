using System;
using System.Linq;
using System.Threading.Tasks;
using Inventory.Core.Data;
using Inventory.Core.Services;
using Inventory.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Inventory.Tests.Controllers
{
    public class InventoryControllerTests : IDisposable
    {
        private readonly InventoryController _controller;
        private readonly ApiDbContext _context;

        public InventoryControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApiDbContext>()
                .UseInMemoryDatabase(databaseName: nameof(InventoryControllerTests))
                .Options;

            _context = new ApiDbContext(options);
            _context.Database.EnsureCreated();

            var shopService = new ShopService(_context);
            _controller = new InventoryController(shopService);
        }

        [Fact]
        public async Task Get_AllItems_ReturnsOk()
        {
            var actionResult = await _controller.Get();
            var items = actionResult.Value;

            Assert.NotEmpty(items);
        }

        [Fact]
        public async Task Get_ItemsWithValidOffset_ReturnsOk()
        {
            var actionResult = await _controller.Get();
            var allItems = actionResult.Value;

            actionResult = await _controller.Get(offset: 1);
            var filteredItems = actionResult.Value;

            Assert.NotEqual(allItems, filteredItems);
            Assert.Equal(allItems.Count() - 1, filteredItems.Count());
        }

        [Fact]
        public async Task Get_ItemsInvalidOffset_ReturnsBadRequest()
        {
            var actionResult = await _controller.Get(offset: -1);

            Assert.Null(actionResult.Value);
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(1000)]
        public async Task Get_ItemsWithValidLimit_ReturnsOk(int validLimit)
        {
            var actionResult = await _controller.Get(limit: validLimit);
            var filteredItemsCount = actionResult.Value.Count();

            Assert.True(filteredItemsCount <= validLimit);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1001)]
        public async Task Get_ItemsWithInvalidLimit_ReturnsBadRequest(int invalidLimit)
        {
            var actionResult = await _controller.Get(limit: invalidLimit);

            Assert.Null(actionResult.Value);
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        }

        [Theory]
        [InlineData("Dexterity")]
        [InlineData("Elixir")]
        public async Task Get_ItemsWithKnownName_ReturnsOk(string name)
        {
            var actionResult = await _controller.Get(name: name);
            var filteredItems = actionResult.Value;

            Assert.NotEmpty(filteredItems);
        }

        [Fact]
        public async Task Get_ItemsWithUnknownName_ReturnsNoItems()
        {
            var actionResult = await _controller.Get(name: Guid.NewGuid().ToString());
            var filteredItems = actionResult.Value;

            Assert.Empty(filteredItems);
        }

        [Theory]
        [InlineData("Hand")]
        [InlineData("Conjured")]
        public async Task Get_ItemsWithKnownDescription_ReturnsOk(string description)
        {
            var actionResult = await _controller.Get(description: description);
            var filteredItems = actionResult.Value;

            Assert.NotEmpty(filteredItems);
        }

        [Fact]
        public async Task Get_ItemsWithUnknownDescription_ReturnsNoItems()
        {
            var actionResult = await _controller.Get(description: Guid.NewGuid().ToString());
            var filteredItems = actionResult.Value;

            Assert.Empty(filteredItems);
        }

        public void Dispose() => _context.Dispose();
    }
}