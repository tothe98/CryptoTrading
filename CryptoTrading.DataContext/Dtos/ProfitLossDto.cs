using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.DataContext.Dtos
{
    public class ProfitLossDto
    {
        public UserDataDto UserData { get; set; }
        public decimal TotalPNL { get; set; }
        public decimal PnLPercentage { get; set; }
    }

    public class ProfitLossDetailDto
    {
        public string Name { get; set; }
        public string Symbol { get; set; }
        public decimal AvgBuyPricePerUnit { get; set; }
        public decimal Amount { get; set; }
        public decimal PnL { get; set; }
        public decimal PnLPercentage { get; set; }
    }
}
