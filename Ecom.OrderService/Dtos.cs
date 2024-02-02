namespace Ecom.OrderService
{
    public class Dtos
    {
        public record OrderDto(Guid OrderId, Guid UserId, List<OrderItemDto> OrderItems, double Amount);
        public record OrderItemDto(Guid OrderItemId, Guid CatalogItemId,string Name, decimal Price, int Quantity);
        public record CatalogItemDto(Guid Id, string Name, decimal Price);
    }
}
