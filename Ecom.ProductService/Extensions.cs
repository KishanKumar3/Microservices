using Ecom.ProductService.Entities;
using static Ecom.ProductService.Dtos;

namespace Ecom.ProductService
{
    public static class Extensions
    {  
        public static ItemDto AsDto(this Item item)
        {
            return new ItemDto(item.Id, item.Name, item.Description, item.Price, item.CreatedDate, item.Category, item.Subcategory, item.Type);
        }
    }
}
