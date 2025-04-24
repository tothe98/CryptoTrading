using CryptoTrading.DataContext.Dtos;
using CryptoTrading.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CryptoTrading.Controllers
{
    [ApiController]
    [Route("crypto")]
    public class CryptoController : ControllerBase
    {
        private readonly ICryptoService _cryptoService;

        public CryptoController(ICryptoService cryptoService)
        {
            _cryptoService = cryptoService;
        }

        [HttpPut("price")]
        public async Task<ActionResult<CryptoDto>> PriceChange([FromBody] PriceChangeDto price)
        {
            try
            {
                var result = await _cryptoService.PriceChange(price);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("price/history/{id}")]
        public async Task<ActionResult<List<CryptoFluctuationDto>>> History(int id)
        {
            return Ok(await _cryptoService.History(id));
        }
    }
}
