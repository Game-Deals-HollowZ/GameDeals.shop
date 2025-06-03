using GameDeals.Services;
using GameDeals.Shared.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameDeals.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class IsThereAnyDealController : ControllerBase
    {
        private readonly IsThereAnyDealService _service;

        public IsThereAnyDealController(IsThereAnyDealService service)
        {
            _service = service;
        }

        // GET api/IsThereAnyDeal/uuid?title=GameTitle
        [HttpGet("uuid")]
        public async Task<IActionResult> GetGameUuid([FromQuery] string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return BadRequest("Title parameter is required.");

            var uuid = await _service.GetGameUuidAsync(title);
            if (uuid == null)
                return NotFound();

            return Ok(uuid);
        }

        // GET api/IsThereAnyDeal/prices/{uuid}
        [HttpGet("prices/{uuid}")]
        public async Task<IActionResult> GetPrices(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return BadRequest("UUID parameter is required.");

            var prices = await _service.GetPricesAsync(uuid);
            return Ok(prices);
        }

        // GET api/IsThereAnyDeal/pricesByTitle?title=GameTitle
        [HttpGet("pricesByTitle")]
        public async Task<IActionResult> GetPricesByTitle([FromQuery] string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return BadRequest("Title parameter is required.");

            var prices = await _service.GetPricesByTitleAsync(title);
            return Ok(prices);
        }
    }
}
