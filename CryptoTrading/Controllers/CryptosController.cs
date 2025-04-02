using CryptoTrading.DataContext.Dtos;
using CryptoTrading.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace CryptoTrading.Controllers
{

    [ApiController]
    [Route("/cryptos")]
    public class CryptosController : ControllerBase
    {

        private readonly ICryptoService _cryptoService;
        public CryptosController(ICryptoService cryptoService)
        {
            _cryptoService = cryptoService;
        }

        [HttpGet]
        public async Task<ActionResult<List<CryptoDto>>> GetAll()
        {
            return await _cryptoService.GetAllCryptos();

        }

        [HttpPost]
        public async Task<ActionResult<CryptoDto>> Create([FromBody] CryptoCreateDto cryptoCreateDto)
        {
            try
            {
                var result = await _cryptoService.CreateCrypto(cryptoCreateDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CryptoDto>> GetCrypto(int id)
        {
            try
            {
                return Ok(await _cryptoService.GetCrypto(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _cryptoService.DeleteCrypto(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

       
    }
}
