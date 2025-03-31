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
}
