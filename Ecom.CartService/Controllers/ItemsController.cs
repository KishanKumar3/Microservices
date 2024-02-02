using Ecom.CartService.Clients;
using Ecom.CartService.Entities;
using Ecom.Common;
using Microsoft.AspNetCore.Mvc;
using static Ecom.CartService.Dtos;

namespace Ecom.CartService.Controllers
{
    [ApiController]
    [Route("Items")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<CartItem> cartItemsRepository;
        private readonly IRepository<CatalogItem> catalogItemsRepository;
        public ItemsController(IRepository<CartItem> cartItemsRepository, IRepository<CatalogItem> catalogItemsRepository)
        {
            this.cartItemsRepository = cartItemsRepository;
            this.catalogItemsRepository = catalogItemsRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartItemDto>>> GetAsync(Guid userId)
        {
            if(userId == Guid.Empty) return BadRequest();

            var cartItemEntities = (await cartItemsRepository.GetAllAsync(item => item.UserId == userId));
            var itemIds = cartItemEntities.Select(item => item.CatalogItemId);
            var catalogItemEntities = await catalogItemsRepository.GetAllAsync(item => itemIds.Contains(item.Id));

            var cartItemDtos = cartItemEntities.Select(cartItem =>
            {
                var catalogItem = catalogItemEntities.Single(catalogItem => catalogItem.Id == cartItem.CatalogItemId);
                return cartItem.AsDto(catalogItem.Name, catalogItem.Description, catalogItem.Price);
            });

            return Ok(cartItemDtos);
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(GrantItemsDto grantItemsDto)
        {
            var cartItem = await cartItemsRepository.GetAsync(item => item.UserId == grantItemsDto.UserId && item.CatalogItemId == grantItemsDto.CatalogItemId);

            if(cartItem == null)
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

            foreach (var cartItem in cartItems)
            {
                await cartItemsRepository.RemoveAsync(cartItem.Id);
            }

            return Ok();
        }


    }
}
