using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.DataContext.Entities
{
    public class AlertLog : AbstractEntity
    {
        public int AlertId { get; set; }
        public DateTime TriggeredAt { get; set; }
        public string Message { get; set; }
        public Alert Alert { get; set; }
    }
}
