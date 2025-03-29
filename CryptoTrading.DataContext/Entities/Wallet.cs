using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.DataContext.Entities
{
    [Table("Wallets")]
    public class Wallet : AbstractEntity
    {
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public decimal Balance { get; set; } = 0;


        /*
         * Lehetséges további mezők:
         * public string Currency { get; set; } 
         * Egy usernek több pénztárca más-más valutában        
         */

        [NotMapped]
        public User User { get; set; }
    }
}
