using Ecom.OrderService.Entities;
using static Ecom.OrderService.Dtos;

namespace Ecom.OrderService
{
    public static class Extensions
    {

        public static CatalogItemDto AsDto(this CatalogItem item)
        {
            return new CatalogItemDto(item.Id, item.Name, item.Price);
        }
    }
}
