using CryptoTrading.DataContext.Enums;
using CryptoTrading.DataContext;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CryptoTrading.DataContext.Entities;

namespace CryptoTrading.Services.Services
{
    public class SavingLockBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public SavingLockBackgroundService(IServiceScopeFactory scopeFactory)
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
                        var savingLocks = await dbContext.SavingLocks.ToListAsync();
                        foreach (var lockItem in savingLocks)
                        {
                            if (lockItem.IsActive)
                            {
                                decimal dailyInterestFactor = 1 + (lockItem.InterestRate / 100);
                                lockItem.TotalPrice = lockItem.TotalPrice * (1 + lockItem.InterestRate / 100);
                                if (lockItem.EndDate.Date <= DateTime.UtcNow.Date)
                                {
                                    lockItem.IsActive = false;
                                    var userwallett = await dbContext.Wallets.FirstOrDefaultAsync(u => u.UserId == lockItem.UserId);
                                    var userwalettcrypto = await dbContext.WalletHoldings.FirstOrDefaultAsync(u => u.WalletId == userwallett.Id && u.CryptoCurrencyId == lockItem.CryptoCurrencyId);
                                    if (userwalettcrypto != null)
                                    {
                                        userwalettcrypto.Amount += lockItem.TotalPrice * (1 + lockItem.InterestRate / 100);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Hiba InterestRateWatcherBackgroundService: Nem található a saving crypto a felhasználónál");
                                    }
                                }

                            }


                        }


                        await dbContext.SaveChangesAsync(stoppingToken);

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
            }
        }
    }
}