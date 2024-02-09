using Ecom.Common;
using Ecom.OrderService.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Linq.Expressions;
using static Ecom.OrderService.Dtos;

namespace Ecom.OrderService.Controllers
{
    [ApiController]
    [Route("Orders")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IRepository<Order> orderRepository;

        private readonly IRepository<CatalogItem> catalogItemRepository;

        public OrdersController(IRepository<Order> orderRepository, IRepository<CatalogItem> catalogItemRepository)
        {
            this.orderRepository = orderRepository;
            this.catalogItemRepository = catalogItemRepository;
        }

        private Guid GetUserIdFromClaims()
        {
            // Get userId from "user-id" claim
            var userIdClaim = HttpContext.User.FindFirst("user-id");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                // Invalid or missing userId claim
                throw new InvalidOperationException("Invalid or missing user ID claim.");
            }

            return userId;
        }

        [HttpGet]
        public async Task<ActionResult<OrderDetailDto>> GetByIdAsync()
        {
            var userId = GetUserIdFromClaims();

            try
            {
                var orders = await GetOrdersOfUser(userId);
                return Ok(orders);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

        }

        private async Task<List<OrderDetailDto>> GetOrdersOfUser(Guid userId)
        {
            var orders = (await orderRepository.GetAllAsync(order => order.UserId == userId)).ToList();
            if(orders == null)
            {
                throw new KeyNotFoundException();
            }
            SortedSet<Guid> uniqueProductIds = new SortedSet<Guid>();
            Dictionary<Guid, OrderItemDto> items = new Dictionary<Guid, OrderItemDto>();
            List<OrderDetailDto> resultList = new List<OrderDetailDto>();
            foreach(var order in orders)
            {
                var orderItems = order.OrderItems;
                foreach(var orderItem in orderItems)
                {
                    if (uniqueProductIds.Contains(orderItem.CatalogItemId))
                    {
                        OrderItemDto orderItemDto = items[orderItem.CatalogItemId];
                        OrderDetailDto orderDetails = new OrderDetailDto(order.Id, order.UserId, orderItem.CatalogItemId, orderItemDto.Name, orderItemDto.Price, orderItemDto.Quantity, order.OrderDate);
                        resultList.Add(orderDetails);
                    }
                    
                    else
                    {
                        CatalogItemDto catalogItem = (await catalogItemRepository.GetAsync(orderItem.CatalogItemId)).AsDto();
                        OrderItemDto orderItemDto = new OrderItemDto(order.Id, catalogItem.Id, catalogItem.Name, catalogItem.Price, orderItem.Quantity);
                        items.Add(orderItem.CatalogItemId, orderItemDto);
                        uniqueProductIds.Add(orderItem.CatalogItemId);
                        OrderDetailDto orderDetails = new OrderDetailDto(order.Id, order.UserId, orderItem.CatalogItemId, orderItemDto.Name, orderItemDto.Price, orderItemDto.Quantity, order.OrderDate);
                        resultList.Add(orderDetails);
                    }
                }
            }

            return resultList;
        }
    }
}
