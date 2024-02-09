

using Ecom.Common;
using Ecom.Inventory.Contracts;
using Ecom.Inventory.Entities;
using MassTransit;
using MassTransit.Initializers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;


namespace Ecom.Inventory.Controllers
{
    [ApiController]
    [Route("Inventory")]
    [Authorize(Roles = "admin")]
    public class InventoryController : ControllerBase
    {
        private readonly ILogger<InventoryController> _logger;
        private readonly IRepository<InventoryItem> _inventoryRepository;
        private readonly IPublishEndpoint publishEndpoint;

        public InventoryController(ILogger<InventoryController> logger, IRepository<InventoryItem> inventoryRepository, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _inventoryRepository = inventoryRepository;
            this.publishEndpoint = publishEndpoint;
        }

        [HttpGet("{productId}")]
        public async Task<ActionResult<InventoryItemDto>> GetAsync(Guid productId)
        {
            var inventoryItem = await _inventoryRepository.GetAsync(item => item.ProductId == productId);
            if (inventoryItem == null)
            {
                return NotFound();
            }
            var inventoryItemDto = inventoryItem.AsDto();
            return Ok(inventoryItemDto);
        }

        [HttpPut("{productId}")]
        public async Task<ActionResult<InventoryItemDto>> UpdateItemQuantity(Guid productId, UpdateInventoryItemQuantityDto updateInventoryItemQuantity)
        {
            var inventoryItem = await _inventoryRepository.GetAsync(item => item.ProductId == productId);
            if (inventoryItem == null)
            {
                var newInventoryItem = new InventoryItem
                {
                    Id = inventoryItem.Id,
                    Quantity = updateInventoryItemQuantity.Quantity,
                    ProductId = productId,
                    LastUpdated = DateTimeOffset.UtcNow
                };
                await _inventoryRepository.CreateAsync(newInventoryItem);

                await publishEndpoint.Publish(new InventoryUpdatedItem(newInventoryItem.Id,newInventoryItem.ProductId, newInventoryItem.Quantity));
                return Ok(newInventoryItem.AsDto());
                // return Ok((await _inventoryRepository.GetAsync(newInventoryItem.ProductId)).AsDto());
            }
            else
            {
                var newInventoryItem = new InventoryItem
                {
                    Id = inventoryItem.Id,
                    Quantity = updateInventoryItemQuantity.Quantity,
                    ProductId = inventoryItem.ProductId,
                    LastUpdated = DateTimeOffset.UtcNow
                };
                await _inventoryRepository.UpdateAsync(newInventoryItem);
                await publishEndpoint.Publish(new InventoryUpdatedItem(newInventoryItem.Id, newInventoryItem.ProductId, newInventoryItem.Quantity));
            }
            return Ok((await _inventoryRepository.GetAsync(item => item.ProductId == productId)).AsDto());
        }
    }
}