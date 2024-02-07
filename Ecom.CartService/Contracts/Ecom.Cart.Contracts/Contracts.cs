
namespace Ecom.Cart.Contracts
{
    public record PlaceOrderItems(List<OrderItemDto> cartItems, decimal totalAmount, Guid userId);
}