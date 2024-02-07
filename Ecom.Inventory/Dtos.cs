namespace Ecom.Inventory
{
    public record InventoryItemDto(Guid ProductId, int Quantity, DateTimeOffset LastUpdatedAt);

    public record UpdateInventoryItemQuantityDto(int Quantity);

    
}