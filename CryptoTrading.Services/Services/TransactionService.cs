using AutoMapper;
using CryptoTrading.DataContext;
using CryptoTrading.DataContext.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Services
{
    public interface ITransactionService
    {
        Task<List<TransactionDto>> GetTransactions(int userid);
        Task<TransactionDetailDto> GetTransactionDetail(int transactionid);
    }
    public class TransactionService : ITransactionService
    {
        private readonly SQL _context;
        private readonly IMapper _mapper;
        public TransactionService(SQL context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<TransactionDetailDto> GetTransactionDetail(int transactionid)
        {
            var result = await _context.Transactions.Include(t => t.CryptoCurrency).Include(u => u.User).FirstOrDefaultAsync(t => t.Id == transactionid);
            if (result == default)
            {
                throw new Exception("Transaction not found");
            }
            return _mapper.Map<TransactionDetailDto>(result);
        }

        public async Task<List<TransactionDto>> GetTransactions(int userid)
        {
            List<TransactionDto> transactions = new List<TransactionDto>();
            var result = await _context.Transactions.Where(t => t.UserId == userid).ToListAsync();
            result.ForEach(transaction =>
            {
                transactions.Add(_mapper.Map(transaction, new TransactionDto()));
            });

            return transactions;
        }
    }
}
