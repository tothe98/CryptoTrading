using CryptoTrading.DataContext.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.DataContext.Dtos
{
    public class CryptoDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public decimal CurrentPrice { get; set; }
    }

    public class CryptoCreateDto
    {
        public string Name { get; set; }
        public string Symbol { get; set; }
        public decimal InitialPrice { get; set; }
    }

    public class CryptoBuySellDto
    {
        public int UserId { get; set; }
        public int CryptoId { get; set; }
        public int Amount { get; set; }
    }

    public class TransactionDto
    {
        public int Id { get; set; }
        public int UsertId { get; set; }
        public int CryptoId { get; set; }
        public ETransactionType TransactionType { get; set; }
        public int Quantity { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
