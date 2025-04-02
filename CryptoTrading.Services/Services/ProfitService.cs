using AutoMapper;
using CryptoTrading.DataContext;
using CryptoTrading.DataContext.Dtos;
using CryptoTrading.DataContext.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Services
{
    public interface IProfitService
    {
        Task<ProfitLossDto> GetProfitLoss(int userid);
        Task<List<ProfitLossDetailDto>> GetDetailPorfitLoss(int userid);
    }
    public class ProfitService : IProfitService
    {
        private readonly SQL _context;
        private readonly IMapper _mapper;

        public ProfitService(SQL context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ProfitLossDto> GetProfitLoss(int userid)
        {
            decimal ProfitLossValue = 0;
            decimal currentPriceSum = 0;
            decimal holdPriceSum = 0;
            var cryptos = await _context.Wallets.Include(w => w.Holdings).ThenInclude(h => h.CryptoCurrency).Where(c => c.UserId == userid).FirstOrDefaultAsync();
            if (cryptos == default)
            {
                throw new Exception("User not found");
            }

            foreach (var crypto in cryptos.Holdings)
            {

                decimal currentPriceForAll = crypto.CryptoCurrency.CurrentPrice * crypto.Amount;
                currentPriceSum += currentPriceForAll;
                var transactions = await _context.Transactions.Where(t => t.CryptoCurrencyId == crypto.CryptoCurrencyId && t.UserId == userid).ToListAsync();
                var sumBuyTransaction = transactions.Where(t => t.TransactionType == ETransactionType.BUY).Sum(item => item.TotalPrice);
                var sumSellTransaction = transactions.Where(t => t.TransactionType == ETransactionType.SELL).Sum(item => item.TotalPrice);
                holdPriceSum += (sumBuyTransaction - sumSellTransaction);
                ProfitLossValue += (currentPriceForAll - (sumBuyTransaction - sumSellTransaction));

            }

            return new ProfitLossDto()
            {
                UserData = _mapper.Map(await _context.Users.FirstOrDefaultAsync(u => u.Id == userid), new UserDataDto()),
                TotalPNL = ProfitLossValue,
                PnLPercentage = Math.Round((1 - (currentPriceSum / holdPriceSum)) * 100, 2)
            };
        }

        public async Task<List<ProfitLossDetailDto>> GetDetailPorfitLoss(int userid)
        {
            List<ProfitLossDetailDto> result = new List<ProfitLossDetailDto>();
            var cryptos = await _context.Wallets.Include(w => w.Holdings).ThenInclude(h => h.CryptoCurrency).Where(c => c.UserId == userid).FirstOrDefaultAsync();
            if (cryptos == default)
            {
                throw new Exception("User not found");
            }

            foreach (var crypto in cryptos.Holdings)
            {
                decimal ProfitLossValue = 0;
                decimal currentPriceSum = 0;
                decimal holdPriceSum = 0;

                decimal currentPriceForAll = crypto.CryptoCurrency.CurrentPrice * crypto.Amount;
                currentPriceSum += currentPriceForAll;
                var transactions = await _context.Transactions.Where(t => t.CryptoCurrencyId == crypto.CryptoCurrencyId && t.UserId == userid).ToListAsync();
                var sumBuyTransaction = transactions.Where(t => t.TransactionType == ETransactionType.BUY).Sum(item => item.TotalPrice);
                var sumSellTransaction = transactions.Where(t => t.TransactionType == ETransactionType.SELL).Sum(item => item.TotalPrice);
                holdPriceSum += (sumBuyTransaction - sumSellTransaction);
                ProfitLossValue = (currentPriceForAll - (sumBuyTransaction - sumSellTransaction));

                result.Add(new ProfitLossDetailDto()
                {
                    Name = crypto.CryptoCurrency.Name,
                    Symbol = crypto.CryptoCurrency.Symbol,
                    Amount = crypto.Amount,
                    PnL = ProfitLossValue,
                    AvgBuyPricePerUnit = (sumBuyTransaction-sumSellTransaction) / crypto.Amount,
                    PnLPercentage = Math.Round((1 - (currentPriceSum / holdPriceSum)) * 100, 2)

                });

            }

            return result;
        }

    }
}
