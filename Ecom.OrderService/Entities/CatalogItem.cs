using Ecom.Common;

namespace Ecom.OrderService.Entities
{
    public class CatalogItem : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
