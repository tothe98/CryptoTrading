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

        [HttpPut("fees")]
        public async Task<ActionResult<FeeDto>> NewFee([FromBody] CreateFee fee)
        {
            try
            {
                if (fee.NewFee > 50 || fee.NewFee < 0)
                {
                    return BadRequest("A fee-nek 0 és 50 között kell lennie!");
                }
                else
                {
                    return await _transactionService.NewFee(fee.NewFee);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("/fees/{userId}")]
        public async Task<ActionResult<FeeStat>> GetFee(int userId)
        {
            return Ok(await _transactionService.GetFees(userId));
        }


    }
}
