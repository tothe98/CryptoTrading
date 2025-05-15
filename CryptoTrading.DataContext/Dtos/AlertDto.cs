using CryptoTrading.DataContext.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.DataContext.Dtos
{
    public class AlertDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CryptoCurrencyId { get; set; }
        public decimal Price { get; set; } = 0;
        public ELimitType LimitType { get; set; }

    }

    public class CreateAlertDto
    {
        public int UserId { get; set; }
        public int CryptoCurrencyId { get; set; }
        public decimal Price { get; set; } = 0;
        public ELimitType LimitType { get; set; }
    }

    public class AlertDetailsDto
    {
        public int Id { get; set; }
        public CryptoDto CryptoCurrency { get; set; }
        public decimal Price { get; set; }
        public String LimitType { get; set; }
    }
}
