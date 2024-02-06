using Ecom.CartService.Entities;
using static Ecom.CartService.Dtos;

namespace Ecom.CartService
{
    public static class Extensions
    {
        public static CartItemDto AsDto(this CartItem item, string name, string description, decimal price)
        {
            return new CartItemDto(item.CatalogItemId, name, description, price, item.Quantity, item.AcquiredDate);
        }
    }
}
