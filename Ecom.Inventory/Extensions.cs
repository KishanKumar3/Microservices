using Ecom.Inventory.Entities;

namespace Ecom.Inventory
{
    public static class Extensions
    {

        public static InventoryItemDto AsDto(this InventoryItem item)
        {
            return new InventoryItemDto(item.ProductId, item.Quantity, item.LastUpdated);
        }
    }
}