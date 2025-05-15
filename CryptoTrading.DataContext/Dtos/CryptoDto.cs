using CryptoTrading.DataContext.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
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

    public class PriceChangeDto
    {
        public int CryptoId { get; set; }
        public decimal NewPrice { get; set; }
    }

    public class CryptoFluctuationDto
    {
        public string Name { get; set; }
        public decimal OldPrice { get; set; }
        public decimal Price { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }

    public class TransactionDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CryptoCurrencyId { get; set; }
        public String TransactionType { get; set; }
        public decimal Quantity { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class TransactionDetailDto
    {
        public int Id { get; set; }
        public UserDataDto User { get; set; }
        public CryptoDto CryptoCurrency { get; set; }
        public string TransactionType { get; set; }
        public decimal Quantity { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class GiftCryptoDto
    {
        public int SenderUserId { get; set; }
        public int RecipientUserId { get; set; }
        public int CryptoCurrencyId { get; set; }
        public decimal Quantity { get; set; }
    }

    public class GiftHistoryDto
    {
        public string CryptoName { get; set; }
        public decimal Quantity { get; set; }

        public decimal PriceAtGiftTime { get; set; }    // ajándékozáskori ár
        public decimal CurrentPrice { get; set; }       // jelenlegi ár

        public DateTime Timestamp { get; set; }

        public string Direction { get; set; }           // "sent" vagy "received"
    }
}
