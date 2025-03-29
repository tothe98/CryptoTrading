using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.DataContext.Entities
{
    [Table("WalletHoldings")]
    public class WalletHolding : AbstractEntity
    {
        public int WalletId { get; set; }
        [ForeignKey("WalletId")]
        public int CryptoCurrencyId { get; set; }
        [ForeignKey("CryptoCurrencyId")]

        [Required]
        public decimal Amount { get; set; } = 0;

        [NotMapped]
        public Wallet Wallet { get; set; }
        [NotMapped]
        public CryptoCurrency CryptoCurrency { get; set; }

    }
}
