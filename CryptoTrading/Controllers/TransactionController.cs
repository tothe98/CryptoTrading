using CryptoTrading.DataContext.Dtos;
using CryptoTrading.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace CryptoTrading.Controllers
{
    [ApiController]
    [Route("transactions")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet("{userid}")]
        public async Task<ActionResult<List<TransactionDto>>> GetTransaction(int userid)
        {
            var result = await _transactionService.GetTransactions(userid);
            return Ok(result);
        }

        [HttpGet("details/{transactionid}")]
        public async Task<ActionResult<TransactionDetailDto>> GetTransactionDetail(int transactionid)
        {
            try
            {
                var result = await _transactionService.GetTransactionDetail(transactionid);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
