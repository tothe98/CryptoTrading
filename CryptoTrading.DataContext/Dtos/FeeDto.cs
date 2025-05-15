using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.DataContext.Dtos
{
    public class FeeDto
    {
        public decimal FeeValue { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateFee
    {
        public decimal NewFee { get; set; }
    }

    public class FeeStat
    {
        public decimal Total { get; set; }
        public List<TransactionReturnDto> Transactions { get; set; }
        public List<DailyTransactionSummaryDto> DailyTransactionSummaries { get; set; } = new();

    }

    public class DailyTransactionSummaryDto
    {
        public DateTime Date { get; set; }
        public int TransactionCount { get; set; }
        public decimal TotalVolume { get; set; }
        public decimal TotalValue { get; set; }
        public decimal TotalFee { get; set; }
    }
}
