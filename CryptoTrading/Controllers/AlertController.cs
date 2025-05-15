using CryptoTrading.DataContext.Dtos;
using CryptoTrading.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace CryptoTrading.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlertController : ControllerBase
    {
        private readonly IAlertService _alertService;
        public AlertController(IAlertService alertService)
        {
            _alertService = alertService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAlert([FromBody] CreateAlertDto alert)
        {
            try
            {
                var createdAlert = await _alertService.CreateAlert(alert);
                return CreatedAtAction(nameof(CreateAlert), new { id = createdAlert.Id }, createdAlert);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{alertId}")]
        public async Task<IActionResult> DeleteAlert(int alertId)
        {
            try
            {
                var result = await _alertService.DeleteAlert(alertId);
                return NoContent();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{userid}")]
        public async Task<IActionResult> GetAlerts(int userid)
        {
            return Ok(await _alertService.GetAlerts(userid));
        }
    }
}
