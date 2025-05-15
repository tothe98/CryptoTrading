using CryptoTrading.DataContext;
using CryptoTrading.DataContext.Entities;
using CryptoTrading.DataContext.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Services
{
    public class CryptoBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private static readonly Random _random = new();

        public CryptoBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<SQL>();

                        var cryptos = await dbContext.CryptoCurrencies.ToListAsync(stoppingToken);
                        foreach (var crypto in cryptos)
                        {
                            decimal oldPrice = crypto.CurrentPrice;
                            decimal percentageChange = (decimal)(_random.NextDouble() * 2 - 1) * 0.05m; // ±5%
                            decimal newPrice = Math.Max(0.01m, crypto.CurrentPrice * (1 + percentageChange));

                            crypto.CurrentPrice = newPrice;
                            dbContext.CryptoPriceFluctuations.Add(new CryptoPriceFluctuation
                            {
                                CryptoCurrencyId = crypto.Id,
                                Price = newPrice,
                                Timestamp = DateTime.UtcNow,
                                OldPrice = oldPrice
                            });
                        }

                        await dbContext.SaveChangesAsync(stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
