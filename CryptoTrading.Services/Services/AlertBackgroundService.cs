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
    public class AlertBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public AlertBackgroundService(IServiceScopeFactory scopeFactory)
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
                        var alerts = await dbContext.Alerts.Include(c => c.CryptoCurrency).Where(a => a.IsActive).ToListAsync(stoppingToken);
                        foreach (var alert in alerts)
                        {
                            var limitPrice = alert.Price;
                            var cryptoPrice = alert.CryptoCurrency.CurrentPrice;
                            var limitType = alert.LimitType;
                            if (limitType == ELimitType.TOP)
                            {
                                Console.WriteLine("TOP");
                                if (limitPrice < cryptoPrice)
                                {
                                    if (alert.IsTriggered == false)
                                    {
                                        alert.IsTriggered = true;
                                        AlertLog alterLog = new AlertLog()
                                        {
                                            AlertId = alert.Id,
                                            TriggeredAt = DateTime.Now,
                                            Message = "Átlépte a felső korlátot"
                                        };
                                        await dbContext.AlertLogs.AddAsync(alterLog);
                                        await dbContext.SaveChangesAsync();
                                    }

                                }
                                else
                                {
                                    alert.IsTriggered = false;
                                    await dbContext.SaveChangesAsync();
                                }
                            }
                            if (limitType == ELimitType.BOTTOM)
                            {
                                Console.WriteLine("BOTTOM");
                                if (limitPrice > cryptoPrice)
                                {
                                    if (alert.IsTriggered == false)
                                    {
                                        alert.IsTriggered = true;
                                        AlertLog alterLog = new AlertLog()
                                        {
                                            AlertId = alert.Id,
                                            TriggeredAt = DateTime.Now,
                                            Message = "Átlépte az alsó korlátot"
                                        };
                                        await dbContext.AlertLogs.AddAsync(alterLog);
                                        await dbContext.SaveChangesAsync();
                                    }

                                }
                                else
                                {
                                    alert.IsTriggered = false;
                                    await dbContext.SaveChangesAsync();
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