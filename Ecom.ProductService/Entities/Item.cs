using Ecom.Common;

namespace Ecom.ProductService.Entities
{
    public class Item : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTimeOffset CreatedDate { get; set; }

        public string Category { get; set; }
        public string Subcategory { get; set; }
        public string Type { get; set; }

    }
}
