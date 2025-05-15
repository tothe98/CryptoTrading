using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.DataContext.Dtos
{
    public class TransactionReturnDto
    {
        public int TransactionId { get; set; }
        public int UserId { get; set; }
        public int CryptoCurrencyId { get; set; }
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
