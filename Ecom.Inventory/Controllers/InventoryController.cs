

using Ecom.Common;
using Ecom.Inventory.Entities;
using MassTransit.Initializers;
using Microsoft.AspNetCore.Mvc;


namespace Ecom.Inventory.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class InventoryController : ControllerBase
    {
        private readonly ILogger<InventoryController> _logger;
        private readonly IRepository<InventoryItem> _inventoryRepository;

        public InventoryController(ILogger<InventoryController> logger, IRepository<InventoryItem> inventoryRepository)
        {
            _logger = logger;
            _inventoryRepository = inventoryRepository;
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
                    Id = Guid.NewGuid(),
                    Quantity = updateInventoryItemQuantity.Quantity,
                    ProductId = productId,
                    LastUpdated = DateTimeOffset.UtcNow
                };
                await _inventoryRepository.CreateAsync(newInventoryItem);
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
            }
            return Ok((await _inventoryRepository.GetAsync(item => item.ProductId == productId)).AsDto());
        }
    }
}