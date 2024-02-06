using Ecom.Common;

namespace Ecom.OrderService.Entities
{
    public class OrderItem : IEntity
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid CatalogItemId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
