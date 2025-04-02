using CryptoTrading.DataContext.Dtos;
using CryptoTrading.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace CryptoTrading.Controllers
{
    [ApiController]
    [Route("profit")]
    public class ProfitController : ControllerBase
    {
        private readonly IProfitService _profitService;
        public ProfitController(IProfitService profitService)
        {
            _profitService = profitService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProfitLossDto>> AllProfit(int id)
        {
            try
            {
                var result = await _profitService.GetProfitLoss(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("details/{id}")]
        public async Task<ActionResult<List<ProfitLossDetailDto>>> DetailProfit(int id)
        {
            try
            {
                var result = await _profitService.GetDetailPorfitLoss(id);
                return Ok(result);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
