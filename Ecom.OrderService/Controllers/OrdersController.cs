using Ecom.Common;
using Ecom.OrderService.Entities;
using Microsoft.AspNetCore.Mvc;
using static Ecom.OrderService.Dtos;

namespace Ecom.OrderService.Controllers
{
    [ApiController]
    [Route("Orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IRepository<OrderItem> orderItemsRepository;
        private readonly IRepository<CatalogItem> catalogItemsRepository;

        public OrdersController(IRepository<OrderItem> orderItemsRepository, IRepository<CatalogItem> catalogItemsRepository)
        {
            this.orderItemsRepository = orderItemsRepository;
            this.catalogItemsRepository = catalogItemsRepository;
        }
    }
}
