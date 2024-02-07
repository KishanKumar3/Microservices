using Ecom.CartService.Entities;
using Ecom.Common;
using Ecom.Inventory.Contracts;
using MassTransit;

namespace Ecom.CartService.Consumer
{

    public class InventoryItemConsumer : IConsumer<InventoryUpdatedItem>
    {

        private readonly IRepository<InventoryItem> repository;
        public InventoryItemConsumer(IRepository<InventoryItem> repository)
        {
            this.repository = repository;
        }

        public async Task Consume(ConsumeContext<InventoryUpdatedItem> context)
        {
            var message = context.Message;

            var item = await repository.GetAsync(item => item.ProductId == message.ProductId);
            if (item != null)
            {
                await repository.UpdateAsync(new InventoryItem
                {
                    Id = message.Id,
                    ProductId = message.ProductId,
                    Quantity = message.Quantity
                });
            }
            else
            {
                await repository.CreateAsync(new InventoryItem
                {
                    Id = message.Id,
                    ProductId = message.ProductId,
                    Quantity = message.Quantity
                });
            }
        }
    }
}