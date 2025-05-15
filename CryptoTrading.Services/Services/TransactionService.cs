using AutoMapper;
using CryptoTrading.DataContext;
using CryptoTrading.DataContext.Dtos;
using CryptoTrading.DataContext.Entities;
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
        Task<FeeDto> NewFee(decimal newFee);
        Task<FeeStat> GetFees(int userid);
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

        public async Task<FeeStat> GetFees(int userid)
        {
            var allFee = await _context.Transactions.Where(t => t.UserId == userid).Select(t => new TransactionReturnDto()
            {
                CryptoCurrencyId = t.CryptoCurrencyId,
                Fee = t.FeeValue,
                UserId = t.UserId,
                TransactionId = t.Id,
                Timestamp = t.Timestamp,
                Amount = t.PricePerUnit * t.Quantity,
                TotalAmount = t.TotalPrice
            }).ToListAsync();
            var dayFee = await _context.Transactions.Where(t => t.UserId == userid).GroupBy(t => t.Timestamp.Date)
            .Select(g => new DailyTransactionSummaryDto
            {
                Date = g.Key,
                TransactionCount = g.Count(),
                TotalVolume = g.Sum(t => t.Quantity),
                TotalValue = g.Sum(t => t.TotalPrice),
                TotalFee = g.Sum(t => t.FeeValue)
            }).ToListAsync();
            var totalFee = await _context.Transactions.Where(t => t.UserId == userid).SumAsync(t => t.FeeValue);

            return new FeeStat()
            {
                DailyTransactionSummaries = dayFee,
                Total = totalFee,
                Transactions = allFee
            };
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

        public async Task<FeeDto> NewFee(decimal newFee)
        {
            var result = await _context.Fees.AddAsync(new Fee() { FeeValue = newFee });
            await _context.SaveChangesAsync();
            return _mapper.Map<FeeDto>(result.Entity);
        }
    }
}
