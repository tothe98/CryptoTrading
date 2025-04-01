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

    public interface IWalletService
    {
        Task<WalletDetailDto> GetWallet(int id);
        Task<WalletDto> UpdateBalance(int userId, WalletUpdateBalanceDto balance);
        Task<bool> DeleteWallet(int userId);
    }

    public class WalletService : IWalletService
    {
        private readonly SQL _context;
        private readonly IMapper _mapper;
        public WalletService(SQL context, IMapper autoMapperProfile)
        {
            _context = context;
            _mapper = autoMapperProfile;
        }

        public async Task<bool> DeleteWallet(int userId)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
            if (wallet != default)
            {
                try
                {

                    var walletHoldings = _context.WalletHoldings.Where(h => h.WalletId == wallet.Id).ToList();
                    _context.WalletHoldings.RemoveRange(walletHoldings);
                    _context.Wallets.Remove(wallet);
                    await _context.SaveChangesAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Server error: {ex.Message}");
                }
            }
            else
            {
                throw new Exception("Wallet not found");
            }
        }

        public async Task<WalletDetailDto> GetWallet(int id)
        {
            var wallet = await _context.Wallets.Include(h => h.Holdings).ThenInclude(c => c.CryptoCurrency).FirstOrDefaultAsync(w => w.UserId == id);
            if (wallet != default)
            {
                return _mapper.Map(wallet, new WalletDetailDto());
            }
            throw new Exception("Wallet not found");

        }

        public async Task<WalletDto> UpdateBalance(int userId, WalletUpdateBalanceDto balance)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
            if (wallet != default)
            {
                wallet.Balance = balance.Balance;
                await _context.SaveChangesAsync();
                return _mapper.Map(wallet, new WalletDto());
            }
            else
            {
                throw new Exception("Wallet not found");
            }
        }
    }
}
