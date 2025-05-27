using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.DataContext.Entities
{
    public class CryptoInterestRate:AbstractEntity
    {
        public int CryptoCurrencyId { get; set; }
        public decimal InterestRate { get; set; }        
        public CryptoCurrency CryptoCurrency { get; set; }

    }
}
