using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.DataContext.Dtos
{
    public class WalletDto
    {
        public int Id { get; set; }
        public decimal Balance { get; set; }
    }


    public class WalletHoldingDto
    {
        public int Id { get; set; }
        public int WalletId { get; set; }
        public string Crypto { get; set; }
        public decimal Amount { get; set; }
    }
}
