using CryptoTrading.DataContext.Entities;
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

    public class WalletDetailDto
    {
        public int Id { get; set; }
        public decimal Balance { get; set; }
        public List<WalletHoldingDto> Holdings { get; set; }

    }

    public class WalletHoldingDto
    {
        public int CryptoCurrencyId { get; set; }
        public int WalletId { get; set; }
        public CryptoDto CryptoCurrency { get; set; }
        public decimal Amount { get; set; }
    }

    public class WalletHoldingAmountDto
    {
        public CryptoDto CryptoCurrency { get; set; }
        public decimal Amount { get; set; }
    }

    public class WalletUpdateBalanceDto
    {
        public decimal Balance { get; set; }
    }
}
