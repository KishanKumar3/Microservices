using Ecom.Cart.Contracts;
using Ecom.Common;
using Ecom.OrderService.Entities;
using MassTransit;
using Microsoft.AspNetCore.Http.Features;
// using Ecom.

namespace Ecom.OrderService.Consumers
{
    public class OrderPlaceItemsConsumer : IConsumer<PlaceOrderItems>
    {
        private readonly IRepository<OrderItem> orderItemsRepository;
        private readonly IRepository<CatalogItem> catalogItemsRepository;
        private readonly IRepository<Order> ordersRepository;

        public OrderPlaceItemsConsumer(IRepository<OrderItem> orderItemsRepository, IRepository<CatalogItem> catalogItemsRepository, IRepository<Order> ordersRepository)
        {
            this.orderItemsRepository = orderItemsRepository;
            this.catalogItemsRepository = catalogItemsRepository;
            this.ordersRepository = ordersRepository;
        }
        public async Task Consume(ConsumeContext<PlaceOrderItems> context)
        {
            var message = context.Message;
            var totalAmount = message.totalAmount;
            var buyer = message.userId;
            var newOrderId = Guid.NewGuid();
            var orders = new Order
            {
                Id = newOrderId,
                Amount = totalAmount,
                UserId = buyer
            };
            foreach (var orderItem in message.cartItems)
            {
                var productId = orderItem.CatalogItemId;
                var itemBuyingPrice = orderItem.Price;
                var orderItemEntity = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    CatalogItemId = productId,
                    OrderId = newOrderId,
                    Price = itemBuyingPrice,
                    Quantity = orderItem.Quantity
                };
                orders.OrderItems.Add(orderItemEntity);
                await catalogItemsRepository.CreateAsync(new CatalogItem
                {
                    Id = productId,
                    Name = orderItem.Name,
                    Price = orderItem.Price
                });
                // await repository.UpdateAsync(new InventoryItem
                // {
                //     Id = inventoryItem.Id,
                //     LastUpdated = DateTimeOffset.UtcNow,
                //     ProductId = productId,
                //     Quantity = inventoryItem.Quantity - orderItem.Quantity
                // });
            }
        }
    }
}