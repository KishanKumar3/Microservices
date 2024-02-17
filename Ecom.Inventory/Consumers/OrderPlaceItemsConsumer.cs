using Ecom.Cart.Contracts;
using Ecom.Common;
using Ecom.Inventory.Contracts;
using Ecom.Inventory.Entities;
using MassTransit;
using Microsoft.AspNetCore.Http.Features;
// using Ecom.

namespace Ecom.Inventory.Consumers
{
    public class OrderPlaceItemsConsumer : IConsumer<PlaceOrderItems>
    {
        private readonly IRepository<InventoryItem> repository;
        private readonly IPublishEndpoint publishEndpoint;

        public OrderPlaceItemsConsumer(IRepository<InventoryItem> repository, IPublishEndpoint publishEndpoint)
        {
            this.repository = repository;
            this.publishEndpoint = publishEndpoint;

        }
        public async Task Consume(ConsumeContext<PlaceOrderItems> context)
        {
            var message = context.Message;

            foreach (var orderItem in message.cartItems)
            {
                var productId = orderItem.CatalogItemId;
                var buyQuantity = orderItem.Quantity;
                var availableQuantity = (await repository.GetAsync(invItem => invItem.ProductId == productId)).Quantity;
                if (availableQuantity < buyQuantity)
                {
                    throw new InvalidOperationException("The stock is less than required.");
                }
            }
            foreach (var orderItem in message.cartItems)
            {
                var productId = orderItem.CatalogItemId;
                var buyQuantity = orderItem.Quantity;
                var inventoryItem = await repository.GetAsync(invItem => invItem.ProductId == productId);
                await repository.UpdateAsync(new InventoryItem
                {
                    Id = inventoryItem.Id,
                    LastUpdated = DateTimeOffset.UtcNow,
                    ProductId = inventoryItem.ProductId,
                    Quantity = inventoryItem.Quantity - orderItem.Quantity
                });

                await publishEndpoint.Publish(new InventoryUpdatedItem(inventoryItem.Id,inventoryItem.ProductId, inventoryItem.Quantity - orderItem.Quantity));
            }
        }
    }
}