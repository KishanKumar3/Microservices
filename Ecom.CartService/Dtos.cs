using System.Globalization;

namespace Ecom.CartService
{
    public class Dtos
    {
        public record GrantItemsDto(Guid CatalogItemId, int Quantity);
        public record CartItemDto(Guid CatalogItemId, string Name, string Description, decimal Price, int Quantity, DateTimeOffset AcquiredDate);

        public record CatalogItemDto(Guid Id, string Name, string Description, decimal Price);
        
    }
}
