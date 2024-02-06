namespace Ecom.Cart.Contracts
{
    public record OrderItemDto(Guid CatalogItemId, string Name, string Description, decimal Price, int Quantity, DateTimeOffset AcquiredDate);
}