using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.DataContext.Dtos
{
    public class PortfolioDto
    {
        public decimal TotalPrice { get; set; }
        public decimal Balance { get; set; }
        public List<WalletHoldingAmountDto> Holdings { get; set; }
    }
}
