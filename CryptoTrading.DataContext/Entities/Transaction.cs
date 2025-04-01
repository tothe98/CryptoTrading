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
        public int UsertId { get; set; }
        public int CryptoId { get; set; }
        public ETransactionType TransactionType { get; set; }
        public int Quantity { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
