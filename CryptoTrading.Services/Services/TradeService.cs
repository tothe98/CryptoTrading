using AutoMapper;
using CryptoTrading.DataContext;
using CryptoTrading.DataContext.Entities;
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
    public interface ITradeService
    {
        Task<TransactionDto> BuyCrypto(CryptoBuySellDto crypto);
        Task<TransactionDto> SellCrypto(CryptoBuySellDto crypto);
    }
    public class TradeService : ITradeService
    {
        private readonly SQL _context;
        private readonly IMapper _mapper;

        public TradeService(SQL context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<TransactionDto> BuyCrypto(CryptoBuySellDto buy)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == buy.UserId);
            if (user == default)
            {
                throw new Exception("User not found");
            }
            var userWallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == buy.UserId);
            if (userWallet == default)
            {
                throw new Exception("Wallet not found");

            }
            var crypto = await _context.CryptoCurrencies.FirstOrDefaultAsync(c => c.Id == buy.CryptoId);
            if (crypto == default)
            {
                throw new Exception("Crypto not found");
            }
            if (crypto.CurrentPrice * buy.Amount > userWallet.Balance)
            {
                throw new Exception("Not enough balance");
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {


                    var holdings = await _context.WalletHoldings.FirstOrDefaultAsync(h => h.CryptoCurrencyId == buy.CryptoId);
                    if (holdings == default)
                    {
                        holdings = new WalletHolding()
                        {
                            Amount = buy.Amount,
                            CryptoCurrencyId = buy.CryptoId,
                            WalletId = userWallet.Id,
                        };
                        _context.WalletHoldings.Add(holdings);
                    }
                    else
                    {
                        holdings.Amount += buy.Amount;
                    }
                    userWallet.Balance -= crypto.CurrentPrice * buy.Amount;
                    await _context.SaveChangesAsync();

                    var transactionEntity = new Transaction()
                    {
                        UserId = user.Id,
                        CryptoCurrencyId = buy.CryptoId,
                        PricePerUnit = crypto.CurrentPrice,
                        Quantity = buy.Amount,
                        TransactionType = ETransactionType.BUY,
                        TotalPrice = crypto.CurrentPrice * buy.Amount
                    };

                    await _context.Transactions.AddAsync(transactionEntity);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return _mapper.Map(transactionEntity, new TransactionDto());
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception($"Server error: {ex.Message}");
                }
            }
        }

        public async Task<TransactionDto> SellCrypto(CryptoBuySellDto sell)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == sell.UserId);
            if (user == default)
            {
                throw new Exception("User not found");
            }
            var userWallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == sell.UserId);
            if (userWallet == default)
            {
                throw new Exception("Wallet not found");

            }
            var crypto = await _context.CryptoCurrencies.FirstOrDefaultAsync(c => c.Id == sell.CryptoId);
            if (crypto == default)
            {
                throw new Exception("Crypto not found");
            }
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {

                    var holdings = await _context.WalletHoldings.FirstOrDefaultAsync(h => h.CryptoCurrencyId == sell.CryptoId);
                    if (holdings == default)
                    {
                        throw new Exception("Not enough quantity");
                    }
                    if (holdings.Amount < sell.Amount)
                    {
                        throw new Exception("Not enough balance");
                    }
                    holdings.Amount -= sell.Amount;

                    userWallet.Balance += crypto.CurrentPrice * sell.Amount;
                    await _context.SaveChangesAsync();

                    var transactionEntity = new Transaction()
                    {
                        UserId = user.Id,
                        CryptoCurrencyId = sell.CryptoId,
                        PricePerUnit = crypto.CurrentPrice,
                        Quantity = sell.Amount,
                        TransactionType = ETransactionType.SELL,
                        TotalPrice = crypto.CurrentPrice * sell.Amount
                    };

                    await _context.Transactions.AddAsync(transactionEntity);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return _mapper.Map(transactionEntity, new TransactionDto());
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception($"Server error: {ex.Message}");
                }
            }
        }
    }
}
