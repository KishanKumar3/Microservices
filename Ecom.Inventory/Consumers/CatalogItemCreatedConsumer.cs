using Ecom.Common;
using Ecom.Inventory.Contracts;
using Ecom.Inventory.Entities;
using MassTransit;
using static Ecom.ProductService.Contracts.Contracts;

namespace Ecom.CartService.Consumers
{
    public class CatalogItemCreatedConsumer : IConsumer<CatalogItemCreated>
    {
        private readonly IRepository<InventoryItem> repository;
        private readonly IPublishEndpoint publishEndpoint;
        public CatalogItemCreatedConsumer(IRepository<InventoryItem> repository, IPublishEndpoint publishEndpoint)
        {
            this.repository = repository;
            this.publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<CatalogItemCreated> context)
        {
            var message = context.Message;

            var item = new InventoryItem
            {
                LastUpdated = DateTimeOffset.UtcNow,
                ProductId = message.ItemId,
                Quantity = 0
            };

            await repository.CreateAsync(item);

            await publishEndpoint.Publish(new InventoryUpdatedItem(
                item.Id,item.ProductId, item.Quantity
            ));
        }
    }
}
