using Ecom.OrderService.Entities;
using static Ecom.OrderService.Dtos;

namespace Ecom.OrderService
{
    public static class Extensions
    {
        public static OrderItemDto AsDto(this OrderItem item, string name , decimal price)
        {
            return new OrderItemDto(item.Id,item.CatalogItemId, name, price, item.Quantity);
        }
    }
}
