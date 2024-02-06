using Ecom.Common;

namespace Ecom.Inventory.Entities
{
    public class InventoryItem : IEntity
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public DateTimeOffset LastUpdated { get; set; }
    }
}