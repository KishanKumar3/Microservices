
using Ecom.Cart.Contracts.Dtos;

namespace Ecom.Cart.Contracts
{
    public record PlaceOrderItems(List<OrderItemDto> cartItems);
}