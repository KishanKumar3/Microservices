// using Ecom.Cart.Contracts;
// using Ecom.Cart.Contracts.Dtos;
using System.Linq.Expressions;
using Ecom.Cart.Contracts;
using Ecom.CartService.Entities;
using Ecom.Common;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver.Core.Operations;
using static Ecom.CartService.Dtos;

namespace Ecom.CartService.Controllers
{
    [ApiController]
    [Route("Items")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<CartItem> cartItemsRepository;
        private readonly IRepository<CatalogItem> catalogItemsRepository;

        private readonly IRepository<InventoryItem> inventoryRepository;

        private readonly IPublishEndpoint publishEndpoint;
        public ItemsController(IRepository<CartItem> cartItemsRepository, IRepository<CatalogItem> catalogItemsRepository, IPublishEndpoint publishEndpoint, IRepository<InventoryItem> inventoryRepository)
        {
            this.cartItemsRepository = cartItemsRepository;
            this.catalogItemsRepository = catalogItemsRepository;
            this.publishEndpoint = publishEndpoint;
            this.inventoryRepository = inventoryRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartItemDto>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty) return BadRequest();

            var cartItemEntities = await cartItemsRepository.GetAllAsync(item => item.UserId == userId);
            var itemIds = cartItemEntities.Select(item => item.CatalogItemId).ToList();
            Expression<Func<CatalogItem, bool>> filter = cartItem => itemIds.Contains(cartItem.Id);
            var catalogItemEntities = await catalogItemsRepository.GetAllAsync(filter);

            var cartItemDtos = cartItemEntities.Select(cartItem =>
            {
                var catalogItem = catalogItemEntities.Single(catalogItem => catalogItem.Id == cartItem.CatalogItemId);
                return cartItem.AsDto(catalogItem.Name, catalogItem.Description, catalogItem.Price);
            }).ToList();
            var totalAmount = cartItemDtos.Sum(cartItem => cartItem.Price * cartItem.Quantity);
            return Ok(new
            {
                items = cartItemDtos,
                totalAmount
            });
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(GrantItemsDto grantItemsDto)
        {
            var inventoryItem = await inventoryRepository.GetAsync(inventoryItem => inventoryItem.ProductId == grantItemsDto.CatalogItemId);
            if (inventoryItem.Quantity < grantItemsDto.Quantity)
            {
                return BadRequest(new
                {
                    message = "Not enough stocks",
                    availableQuantity = inventoryItem.Quantity
                });
            }
            var cartItem = await cartItemsRepository.GetAsync(item => item.UserId == grantItemsDto.UserId && item.CatalogItemId == grantItemsDto.CatalogItemId);

            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    CatalogItemId = grantItemsDto.CatalogItemId,
                    UserId = grantItemsDto.UserId,
                    Quantity = grantItemsDto.Quantity,
                    AcquiredDate = DateTimeOffset.UtcNow
                };

                await cartItemsRepository.CreateAsync(cartItem);
            }
            else
            {
                cartItem.Quantity += grantItemsDto.Quantity;
                await cartItemsRepository.UpdateAsync(cartItem);
            }
            return Ok();
        }

        [HttpDelete("{userId}")]
        public async Task<ActionResult> DeleteCartAsync(Guid userId)
        {
            if (userId == Guid.Empty) return BadRequest();

            var cartItems = await cartItemsRepository.GetAllAsync(item => item.UserId == userId);

            if (cartItems == null || !cartItems.Any())
            {
                return NotFound("Cart not found");
            }

            // Delete each cart item
            foreach (var cartItem in cartItems)
            {
                await cartItemsRepository.RemoveAsync(cartItem.Id);
            }

            return Ok();
        }


        [HttpDelete("{userId}/{catalogItemId}")]
        public async Task<ActionResult> DeleteCartItemAsync(Guid userId, Guid catalogItemId)
        {
            if (userId == Guid.Empty || catalogItemId == Guid.Empty) return BadRequest();

            var cartItem = await cartItemsRepository.GetAsync(item => item.UserId == userId && item.CatalogItemId == catalogItemId);

            if (cartItem == null)
            {
                return NotFound("Cart item not found");
            }

            await cartItemsRepository.RemoveAsync(cartItem.Id);

            return Ok();
        }

        [HttpPost("Checkout/{userId}")]
        public async Task<ActionResult> CheckoutAsync(Guid userId)
        {
            if (userId == Guid.Empty) return BadRequest();

            var cartItems = await cartItemsRepository.GetAllAsync(item => item.UserId == userId);

            if (cartItems == null || !cartItems.Any())
            {
                return NotFound("Cart is empty");
            }

            var cartItemDtos = new List<CartItemDto>();

            foreach (var cartItem in cartItems)
            {
                var catalogItem = await catalogItemsRepository.GetAsync(item => item.Id == cartItem.CatalogItemId);
                cartItemDtos.Add(cartItem.AsDto(catalogItem.Name, catalogItem.Description, catalogItem.Price));
            }

            foreach (var cartItem in cartItems)
            {
                var previousInventoryItem = await inventoryRepository.GetAsync(item => item.ProductId == cartItem.CatalogItemId);
                if (previousInventoryItem.Quantity < cartItem.Quantity)
                {
                    return BadRequest(new
                    {
                        message = "Not enough stocks",
                    });
                }
            }

            foreach (var cartItem in cartItems)
            {
                var previousInventoryItem = await inventoryRepository.GetAsync(item => item.ProductId == cartItem.CatalogItemId);
                await inventoryRepository.UpdateAsync(new InventoryItem
                {
                    Id = previousInventoryItem.Id,
                    ProductId = previousInventoryItem.ProductId,
                    Quantity = previousInventoryItem.Quantity - cartItem.Quantity
                });
                await cartItemsRepository.RemoveAsync(cartItem.Id);
            }

            var totalAmount = cartItemDtos.Sum(item => item.Quantity * item.Price);
            await publishEndpoint.Publish(new PlaceOrderItems(cartItemDtos.Select(cartItem => new OrderItemDto(
                cartItem.CatalogItemId, cartItem.Name, cartItem.Description, cartItem.Price, cartItem.Quantity, cartItem.AcquiredDate
            )).ToList(), totalAmount, userId));

            return Ok();
        }


    }
}
