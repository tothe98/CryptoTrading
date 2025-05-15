using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.DataContext.Entities
{
    public class Fee:AbstractEntity
    {
        public decimal FeeValue { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
