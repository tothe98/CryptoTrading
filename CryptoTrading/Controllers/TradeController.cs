using CryptoTrading.DataContext.Dtos;
using CryptoTrading.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace CryptoTrading.Controllers
{
    [ApiController]
    [Route("trade")]
    public class TradeController : ControllerBase
    {
        private readonly ITradeService _tradeService;

        public TradeController(ITradeService tradeService)
        {
            _tradeService = tradeService;
        }

        [HttpPost("buy")]
        public async Task<ActionResult<TransactionReturnDto>> Buy([FromBody] CryptoBuySellDto crypto)
        {
            try
            {
                var result = await _tradeService.BuyCrypto(crypto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("sell")]
        public async Task<ActionResult<TransactionReturnDto>> Sell([FromBody] CryptoBuySellDto crypto)
        {
            try
            {
                var result = await _tradeService.SellCrypto(crypto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("gift")]
        public async Task<ActionResult<TransactionReturnDto>> Gift([FromBody] GiftCryptoDto crypto)
        {
            try
            {
                var result = await _tradeService.GiftCrypto(crypto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("gifts/{userId}")]
        public async Task<ActionResult<List<GiftHistoryDto>>> GetGiftHistory(int userId)
        {
            var result = await _tradeService.GetGiftHistoryAsync(userId);
            return Ok(result);
        }

    }
}
