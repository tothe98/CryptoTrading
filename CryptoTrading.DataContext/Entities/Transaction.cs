using CryptoTrading.DataContext.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.DataContext.Entities
{
    [Table("TransactionLogs")]
    public class Transaction : AbstractEntity
    {
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public int CryptoCurrencyId { get; set; }
        [ForeignKey("CryptoCurrencyId")]
        public ETransactionType TransactionType { get; set; }
        public decimal Quantity { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public decimal FeeValue { get; set; }

        public CryptoCurrency CryptoCurrency { get; set; }
        public User User { get; set; }
    }
}
