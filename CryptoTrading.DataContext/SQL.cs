using CryptoTrading.DataContext.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.DataContext
{
    public class SQL : DbContext
    {
        public SQL(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<WalletHolding> WalletHoldings { get; set; }
        public DbSet<CryptoCurrency> CryptoCurrencies { get; set; }
        public DbSet<CryptoPriceFluctuation> CryptoPriceFluctuations { get; set; }
    }
}
