using CryptoTrading.DataContext.Dtos;
using CryptoTrading.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace CryptoTrading.Controllers
{
    [ApiController]
    [Route("wallet")]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;
        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WalletDetailDto>> GetWallet(int id)
        {
            try
            {
                var wallet = await _walletService.GetWallet(id);
                return Ok(wallet);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<WalletDto>> UpdateBalance(int id, [FromBody] WalletUpdateBalanceDto balance)
        {
            try
            {
                var wallet = await _walletService.UpdateBalance(id, balance);
                return Ok(wallet);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            try
            {
                await _walletService.DeleteWallet(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
