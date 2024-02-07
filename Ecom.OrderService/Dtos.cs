namespace Ecom.OrderService
{
    public class Dtos
    {
        public record OrderItemDto(Guid OrderItemId, Guid CatalogItemId,string Name, decimal Price, int Quantity);
        public record CatalogItemDto(Guid Id, string Name, decimal Price);

        public record OrderDetailDto(Guid orderId, Guid userId, Guid CatalogItemId, string Name, decimal price, int Quantity, DateTimeOffset orderDate);
    }
}

// orderid, userid, catalogItemdetails, orderDate, price