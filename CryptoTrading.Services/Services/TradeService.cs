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
        Task<TransactionReturnDto> BuyCrypto(CryptoBuySellDto crypto);
        Task<TransactionReturnDto> SellCrypto(CryptoBuySellDto crypto);

        Task<TransactionReturnDto> GiftCrypto(GiftCryptoDto crypto);
        Task<List<GiftHistoryDto>> GetGiftHistoryAsync(int userId);
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

        public async Task<TransactionReturnDto> BuyCrypto(CryptoBuySellDto buy)
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
                    var fee = await _context.Fees.OrderByDescending(f => f.CreatedAt).FirstAsync();
                    userWallet.Balance -= crypto.CurrentPrice * buy.Amount + ((crypto.CurrentPrice * buy.Amount) * fee.FeeValue);
                    await _context.SaveChangesAsync();

                    var transactionEntity = new Transaction()
                    {
                        UserId = user.Id,
                        CryptoCurrencyId = buy.CryptoId,
                        PricePerUnit = crypto.CurrentPrice,
                        Quantity = buy.Amount,
                        TransactionType = ETransactionType.BUY,
                        TotalPrice = (crypto.CurrentPrice * buy.Amount) + ((crypto.CurrentPrice * buy.Amount) * fee.FeeValue),
                        FeeValue = ((crypto.CurrentPrice * buy.Amount) * fee.FeeValue)

                    };

                    var newTransaction = await _context.Transactions.AddAsync(transactionEntity);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    TransactionReturnDto transactionReturnDto = new TransactionReturnDto()
                    {
                        CryptoCurrencyId = crypto.Id,
                        Fee = ((crypto.CurrentPrice * buy.Amount) * fee.FeeValue),
                        UserId = user.Id,
                        TransactionId = newTransaction.Entity.Id,
                        Timestamp = DateTime.Now,
                        Amount = (crypto.CurrentPrice * buy.Amount),
                        TotalAmount = (crypto.CurrentPrice * buy.Amount) + ((crypto.CurrentPrice * buy.Amount) * fee.FeeValue)

                    };
                    return transactionReturnDto;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception($"Server error: {ex.Message}");
                }
            }
        }

        public async Task<TransactionReturnDto> GiftCrypto(GiftCryptoDto dto)
        {
            if (dto.SenderUserId == dto.RecipientUserId)
                throw new InvalidOperationException("Nem ajándékozhatsz magadnak.");

            var senderWallet = await _context.Wallets
                .Include(w => w.Holdings)
                .FirstOrDefaultAsync(w => w.UserId == dto.SenderUserId);

            var recipientWallet = await _context.Wallets
                .Include(w => w.Holdings)
                .FirstOrDefaultAsync(w => w.UserId == dto.RecipientUserId);

            var crypto = await _context.CryptoCurrencies.FindAsync(dto.CryptoCurrencyId);
            if (crypto == null)
                throw new InvalidOperationException("A megadott kriptovaluta nem létezik.");

            var currentPrice = crypto.CurrentPrice;

            var senderHolding = senderWallet.Holdings
                .FirstOrDefault(h => h.CryptoCurrencyId == dto.CryptoCurrencyId);

            if (senderHolding == null || senderHolding.Amount < dto.Quantity)
                throw new InvalidOperationException("Nincs elegendő kriptovalutád az ajándékozáshoz.");

            using var transaction = await _context.Database.BeginTransactionAsync();

            // Levonás a küldőtől
            senderHolding.Amount -= dto.Quantity;

            // Jóváírás a fogadónál
            var recipientHolding = recipientWallet.Holdings
                .FirstOrDefault(h => h.CryptoCurrencyId == dto.CryptoCurrencyId);

            if (recipientHolding == null)
            {
                recipientHolding = new WalletHolding
                {
                    WalletId = recipientWallet.Id,
                    CryptoCurrencyId = dto.CryptoCurrencyId,
                    Amount = 0
                };
                recipientWallet.Holdings.Add(recipientHolding);
            }

            recipientHolding.Amount += dto.Quantity;

            // Tranzakciók naplózása (mindkét félhez – optional)
            _context.Transactions.AddRange(new[]
            {
        new Transaction
        {
            UserId = dto.SenderUserId,
            CryptoCurrencyId = dto.CryptoCurrencyId,
            TransactionType = ETransactionType.GIFT,
            Quantity = dto.Quantity,
            PricePerUnit = currentPrice,
            TotalPrice = currentPrice * dto.Quantity,
            FeeValue = 0
        },
        new Transaction
        {
            UserId = dto.RecipientUserId,
            CryptoCurrencyId = dto.CryptoCurrencyId,
            TransactionType = ETransactionType.GET,
            Quantity = dto.Quantity,
            PricePerUnit = currentPrice,
            TotalPrice = currentPrice * dto.Quantity,
            FeeValue = 0
        }
               });

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            TransactionReturnDto transactionReturnDto = new TransactionReturnDto()
            {
                CryptoCurrencyId = crypto.Id,
                Fee = 0,
                UserId = dto.SenderUserId,
                TransactionId = 1,
                Timestamp = DateTime.Now,
                Amount = 0,
                TotalAmount = 0

            };
            return transactionReturnDto;


        }


        public async Task<TransactionReturnDto> SellCrypto(CryptoBuySellDto sell)
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
                    var fee = await _context.Fees.OrderByDescending(f => f.CreatedAt).FirstAsync();
                    userWallet.Balance += (crypto.CurrentPrice * sell.Amount) - ((crypto.CurrentPrice * sell.Amount) * fee.FeeValue);
                    await _context.SaveChangesAsync();

                    var transactionEntity = new Transaction()
                    {
                        UserId = user.Id,
                        CryptoCurrencyId = sell.CryptoId,
                        PricePerUnit = crypto.CurrentPrice,
                        Quantity = sell.Amount,
                        TransactionType = ETransactionType.SELL,
                        TotalPrice = (crypto.CurrentPrice * sell.Amount) - ((crypto.CurrentPrice * sell.Amount) * fee.FeeValue),
                        FeeValue = ((crypto.CurrentPrice * sell.Amount) * fee.FeeValue)
                    };

                    var newTransaction = await _context.Transactions.AddAsync(transactionEntity);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    TransactionReturnDto transactionReturnDto = new TransactionReturnDto()
                    {
                        CryptoCurrencyId = crypto.Id,
                        Fee = ((crypto.CurrentPrice * sell.Amount) * fee.FeeValue),
                        UserId = user.Id,
                        TransactionId = newTransaction.Entity.Id,
                        Timestamp = DateTime.Now,
                        Amount = (crypto.CurrentPrice * sell.Amount),
                        TotalAmount = (crypto.CurrentPrice * sell.Amount) - ((crypto.CurrentPrice * sell.Amount) * fee.FeeValue)

                    };
                    return transactionReturnDto;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception($"Server error: {ex.Message}");
                }
            }
        }



        public async Task<List<GiftHistoryDto>> GetGiftHistoryAsync(int userId)
        {
            var result = await _context.Transactions
                .Where(t =>
                    (t.TransactionType == ETransactionType.GIFT && t.UserId == userId) ||         
                    (t.TransactionType == ETransactionType.GET && t.UserId == userId)      
                )
                .Include(t => t.CryptoCurrency)
                .Include(t => t.User)
                .Select(t => new GiftHistoryDto
                {
                    CryptoName = t.CryptoCurrency.Name,
                    Quantity = t.Quantity,
                    PriceAtGiftTime = t.PricePerUnit,
                    CurrentPrice = t.CryptoCurrency.CurrentPrice,
                    Timestamp = t.Timestamp,
                    Direction = t.TransactionType == ETransactionType.GIFT ? "sent" : "received",

                })
                .ToListAsync();

            return result;
        }
    }

    }