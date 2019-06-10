using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Inventory.Core.Data;
using Inventory.Core.Services;
using Inventory.Models;
using Inventory.Web.Controllers;
using Inventory.Web.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Inventory.UnitTests
{
    [Trait("Category", "Unit")]
    public class PurchasesControllerTests : IDisposable
    {
        private readonly PurchasesController _controller;
        private readonly ApiDbContext _context;

        private readonly Guid _itemId = Guid.Parse("f0de704d-5d5e-4364-bd01-466f0022e8ff");

        public PurchasesControllerTests()
        {
            var dbName = $"Purchases{Guid.NewGuid().ToString()}";
            var options = new DbContextOptionsBuilder<ApiDbContext>().UseInMemoryDatabase(databaseName: dbName).Options;

            _context = new ApiDbContext(options);
            _context.Items.Add(new Item
            {
                Id = _itemId,
                Name = "Sulfuras, Hand of Ragnaros",
                Price = 10,
                AvailableUnits = 50
            });
            _context.SaveChanges();

            _controller = new PurchasesController(new ShopService(_context));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim("sub", "testuser")
                    }))
                }
            };
        }

        [Fact]
        public async Task Buy_UnknownItem_ReturnsNotFound()
        {
            var request = new PurchaseRequest { ItemId = Guid.NewGuid() };

            var actionResult = await _controller.Post(request);

            Assert.Null(actionResult.Value);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task Buy_ValidRequest_ReturnsCompleted()
        {
            var request = new PurchaseRequest { ItemId = _itemId, Quantity = 5 };

            var actionResult = await _controller.Post(request);
            var purchase = actionResult.Value;

            Assert.NotNull(purchase);
            Assert.Equal(PurchaseStatus.Completed, purchase.Status);
            Assert.Equal(50, purchase.TotalPrice);
        }

        [Fact]
        public async Task Buy_AllAvailableUnits_ReturnsCompleted()
        {
            var request = new PurchaseRequest { ItemId = _itemId, Quantity = 50 };

            var actionResult = await _controller.Post(request);
            var purchase = actionResult.Value;

            Assert.NotNull(purchase);
            Assert.Equal(PurchaseStatus.Completed, purchase.Status);
            Assert.Equal(500, purchase.TotalPrice);
        }

        [Fact]
        public async Task Buy_TooManyItems_ReturnsNotEnoughItems()
        {
            var request = new PurchaseRequest { ItemId = _itemId, Quantity = 51 };

            var actionResult = await _controller.Post(request);
            var purchase = actionResult.Value;

            Assert.NotNull(purchase);
            Assert.Equal(PurchaseStatus.NotEnoughItems, purchase.Status);
            Assert.Equal(0, purchase.TotalPrice);
        }

        [Fact]
        public async Task Buy_AfterAllItemsPurchased_ReturnsOutOfStock()
        {
            var firstRequest = new PurchaseRequest { ItemId = _itemId, Quantity = 50 };
            await _controller.Post(firstRequest);

            var secondRequest = new PurchaseRequest { ItemId = _itemId, Quantity = 1 };
            var actionResult = await _controller.Post(secondRequest);
            var purchase = actionResult.Value;

            Assert.NotNull(purchase);
            Assert.Equal(PurchaseStatus.OutOfStock, purchase.Status);
            Assert.Equal(0, purchase.TotalPrice);
        }

        public void Dispose() => _context.Dispose();
    }
}