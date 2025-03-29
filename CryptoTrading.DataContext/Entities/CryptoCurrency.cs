using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.DataContext.Entities
{
    [Table("CryptoCurrencies")]
    public class CryptoCurrency : AbstractEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Symbol { get; set; }
        [Required]
        public decimal InitialPrice { get; set; } = 0;
    }
}
