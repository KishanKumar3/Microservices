using Ecom.Common;

namespace Ecom.OrderService.Entities
{
    public class Order : IEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
}
