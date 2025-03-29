using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.DataContext.Entities
{
    [Table("CryptoPriceFluctuation")]
    public class CryptoPriceFluctuation :AbstractEntity
    {
        public int CryptoCurrencyId { get; set; }
        [ForeignKey("CryptoCurrencyId")]

        [Required]
        public decimal Price { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;

        [NotMapped]
        public CryptoCurrency CryptoCurrency { get; set; }
    }
}
