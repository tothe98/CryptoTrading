using CryptoTrading.DataContext;
using CryptoTrading.DataContext.Dtos;
using CryptoTrading.DataContext.Entities;
using CryptoTrading.DataContext.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Services
{
    public interface IInitialService
    {
        Task Seed();
    }
    public class InitialService : IInitialService
    {
        private readonly SQL _context;
        private readonly IUserService _userService;
        private readonly ICryptoService _cryptoService;

        public InitialService(SQL context, IUserService userService, ICryptoService cryptoService)
        {
            _context = context;
            _userService = userService;
            _cryptoService = cryptoService;
        }

        public async Task Seed()
        {
            
            if (await _context.CryptoCurrencies.CountAsync() < 15 || await _context.Users.CountAsync() == 0)
            {
                _context.CryptoCurrencies.RemoveRange(_context.CryptoCurrencies);
                _context.CryptoPriceFluctuations.RemoveRange(_context.CryptoPriceFluctuations);
                _context.Transactions.RemoveRange(_context.Transactions);
                _context.Wallets.RemoveRange(_context.Wallets);
                _context.WalletHoldings.RemoveRange(_context.WalletHoldings);
                _context.Users.RemoveRange(_context.Users);
                _context.Fees.RemoveRange(_context.Fees);
                _context.CryptoInterestRates.RemoveRange(_context.CryptoInterestRates);
                await _context.SaveChangesAsync();

                Console.WriteLine("Databased cleard.");


                await _userService.RegisterUser(new UserCreateDto()
                {
                    Username = "admin",
                    Email = "admin@local",
                    Password = "TestPass123",
                    PasswordConfirm = "TestPass123",
                    Role = ERole.ADMIN,
                });

                var cryptos = new[]
                {
                new CryptoCreateDto(){Name = "Bitcoin", Symbol = "BTC", InitialPrice = 28000},
                new CryptoCreateDto() { Name = "Ethereum", Symbol = "ETH", InitialPrice = 1800 },
                new CryptoCreateDto() { Name = "Cardano", Symbol = "ADA", InitialPrice = 0.35m },
                new CryptoCreateDto() { Name = "So lana", Symbol = "SOL", InitialPrice = 25 },
                new CryptoCreateDto() { Name = "Ripple", Symbol = "XRP", InitialPrice = 0.55m },
                new CryptoCreateDto() { Name = "Polkadot", Symbol = "DOT", InitialPrice = 6.5m },
                new CryptoCreateDto() { Name = "Litecoin", Symbol = "LTC", InitialPrice = 95 },
                new CryptoCreateDto() { Name = "Chainlink", Symbol = "LINK", InitialPrice = 7.2m },
                new CryptoCreateDto() { Name = "Avalanche", Symbol = "AVAX", InitialPrice = 14 },
                new CryptoCreateDto() { Name = "Dogecoin", Symbol = "DOGE", InitialPrice = 0.07m },
                new CryptoCreateDto() { Name = "Shiba Inu", Symbol = "SHIB", InitialPrice = 0.000008m },
                new CryptoCreateDto() { Name = "Uniswap", Symbol = "UNI", InitialPrice = 5.5m },
                new CryptoCreateDto() { Name = "Stellar", Symbol = "XLM", InitialPrice = 0.12m },
                new CryptoCreateDto() { Name = "Cosmos", Symbol = "ATOM", InitialPrice = 10 },
                new CryptoCreateDto() { Name = "Tezos", Symbol = "XTZ", InitialPrice = 1.1m },
            };

                foreach (var crypto in cryptos)
                {
                    var created = await _cryptoService.CreateCrypto(crypto);
                    await _context.CryptoInterestRates.AddAsync(new CryptoInterestRate
                    {
                        CryptoCurrencyId = created.Id,
                        InterestRate = 5,
                    });
                }

                await _context.Fees.AddAsync(new DataContext.Entities.Fee()
                {
                    CreatedAt = DateTime.Now,
                    FeeValue = 0.02m
                });

                await _context.SaveChangesAsync();


            }





        }
    }
}
