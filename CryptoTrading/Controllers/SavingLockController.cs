using CryptoTrading.DataContext.Dtos;
using CryptoTrading.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace CryptoTrading.Controllers
{

    [ApiController]
    [Route("savings")]
    public class SavingLockController : ControllerBase
    {
        private readonly ISavingService _savingService;

        public SavingLockController(ISavingService savingService)
        {
            _savingService = savingService;
        }

        [HttpPost("lock")]
        public async Task<ActionResult<ReturnSavingLockDto>> LockAsync([FromBody] CreateSavingLockDto lockDto)
        {
            try
            {
                var created = await _savingService.LockSaving(lockDto);
                return Ok(created);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{userid}")]
        public async Task<ActionResult<List<SavingLockDto>>> List(int userid)
        {
            var result = await _savingService.Get(userid);
            return Ok(result);
        }

        [HttpPut("interest-rate")]
        public async Task<ActionResult<InterestUpdateDto>> UpdateInterest([FromBody] InterestUpdateDto dto)
        {
            try
            {
                var result = await _savingService.UpdateInterest(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("rates")]
        public async Task<ActionResult<List<InterestDto>>> ListInterest()
        {
            return await _savingService.ListInterest();
        }

        [HttpDelete("unlock/{lockid}")]
        public async Task<ActionResult<String>> Delete(int lockid)
        {
            try
            {
                return await _savingService.Delete(lockid);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
