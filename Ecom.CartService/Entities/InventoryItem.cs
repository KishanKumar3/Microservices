using Ecom.Common;

namespace Ecom.CartService.Entities
{
    public class InventoryItem : IEntity
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }
    }
}