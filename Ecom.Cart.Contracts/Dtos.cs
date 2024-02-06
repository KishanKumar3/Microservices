namespace Ecom.Cart.Contracts.Dtos
{
    public record OrderItemDto(Guid CatalogItemId, string Name, string Description, decimal Price, int Quantity, DateTimeOffset AcquiredDate, Guid UserId);
}