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
    public interface ICryptoService
    {
        Task<List<CryptoDto>> GetAllCryptos();
        Task<CryptoDto> CreateCrypto(CryptoCreateDto crypto);
        Task<CryptoDto> GetCrypto(int id);
        Task<bool> DeleteCrypto(int id);
        Task<CryptoDto> PriceChange(PriceChangeDto price);
        Task<List<CryptoFluctuationDto>> History(int id);
    }

    public class CryptoService : ICryptoService
    {
        private readonly SQL _context;
        private readonly IMapper _mapper;

        public CryptoService(SQL context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<CryptoDto>> GetAllCryptos()
        {
            return await _context.CryptoCurrencies.Select(c => _mapper.Map<CryptoDto>(c)).ToListAsync();
        }

        public async Task<CryptoDto> CreateCrypto(CryptoCreateDto crypto)
        {
            if (!await CryptoExists(crypto.Name))
            {
                var newCrypto = _mapper.Map<CryptoCurrency>(crypto);
                await _context.CryptoCurrencies.AddAsync(newCrypto);
                await _context.SaveChangesAsync();
                return _mapper.Map(newCrypto, new CryptoDto());
            }
            throw new Exception("Crypto already exists!");
        }
        public async Task<CryptoDto> GetCrypto(int id)
        {
            var crypto = await _context.CryptoCurrencies.FirstOrDefaultAsync(c => c.Id == id);
            if (crypto != default)
            {
                return _mapper.Map(crypto, new CryptoDto());
            }
            throw new Exception("Crypto not found!");
        }

        public async Task<bool> DeleteCrypto(int id)
        {
            var crypto = await _context.CryptoCurrencies.FirstOrDefaultAsync(c => c.Id == id);
            if (crypto != default)
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        //árváltozás törlése
                        var cryptoFlun = await _context.CryptoPriceFluctuations.Where(c => c.CryptoCurrencyId == id).ToListAsync();
                        _context.CryptoPriceFluctuations.RemoveRange(cryptoFlun);
                        await _context.SaveChangesAsync();
                        //pénztárcából való törlés
                        var walletCrypto = await _context.WalletHoldings.Where(c => c.CryptoCurrencyId == id).ToListAsync();
                        _context.WalletHoldings.RemoveRange(walletCrypto);
                        await _context.SaveChangesAsync();
                        //crypto törlés
                        _context.CryptoCurrencies.Remove(crypto);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception($"Server error: {ex.Message}");
                    }

                }
            }
            throw new Exception("Crypto not found");

        }
        public async Task<CryptoDto> PriceChange(PriceChangeDto price)
        {
            var crypto = await _context.CryptoCurrencies.FirstOrDefaultAsync(c => c.Id == price.CryptoId);
            if (crypto != default)
            {
                try
                {

                    decimal oldPrice = crypto.CurrentPrice;
                    crypto.CurrentPrice = price.NewPrice;
                    var cryptoFluc = new CryptoPriceFluctuation()
                    {
                        CryptoCurrencyId = crypto.Id,
                        OldPrice = oldPrice,
                        Price = price.NewPrice
                    };
                    await _context.CryptoPriceFluctuations.AddAsync(cryptoFluc);
                    await _context.SaveChangesAsync();
                    return _mapper.Map(crypto, new CryptoDto());
                }
                catch (Exception ex)
                {
                    throw new Exception($"Server error: {ex.Message}");
                }

            }
            throw new Exception("Crypto not found");
        }

        public async Task<List<CryptoFluctuationDto>> History(int id)
        {
            List<CryptoFluctuationDto> histories = new List<CryptoFluctuationDto>();
            var cryptoHistory = await _context.CryptoPriceFluctuations.Include(c => c.CryptoCurrency).Where(f => f.CryptoCurrencyId == id).ToListAsync();
            cryptoHistory.ForEach(ch => histories.Add(_mapper.Map(ch, new CryptoFluctuationDto())));
            return histories;
        }

        private async Task<bool> CryptoExists(string name)
        {
            return _context.CryptoCurrencies.Any(c => c.Name == name);
        }

    }
}
