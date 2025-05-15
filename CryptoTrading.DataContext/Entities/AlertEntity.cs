using CryptoTrading.DataContext.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.DataContext.Entities
{
    public class Alert : AbstractEntity
    {
        public int CryptoCurrencyId { get; set; }
        public int UserId { get; set; }
        public decimal Price { get; set; } = 0;
        public ELimitType LimitType { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsTriggered { get; set; } = false;
        public CryptoCurrency CryptoCurrency { get; set; }
        public User User { get; set; }

    }
}
