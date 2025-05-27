using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.DataContext.Entities
{
    public class SavingLock : AbstractEntity
    {
        public int UserId { get; set; }
        public int CryptoCurrencyId { get; set; }
        public decimal Amount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal InterestRate { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsActive { get; set; } = true;

        public User User { get; set; }
        public CryptoCurrency CryptoCurrency { get; set; }
    }
}
