namespace Ecom.Inventory.Contracts
{
    public record InventoryUpdatedItem(Guid Id, Guid ProductId, int Quantity);
}