using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTrading.DataContext.Dtos
{
    public class CreateSavingLockDto
    {
        public int UserId { get; set; }
        public int CryptoCurrencyId { get; set; }
        public decimal Amount { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class ReturnSavingLockDto
    {
        public int UserId { get; set; }
        public int CryptoCurrencyId { get; set; }
        public decimal Amount { get; set; }
        public decimal InterestRate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public decimal ExpectedAmount { get; set; }

    }

    public class SavingLockDto
    {
        public int UserId { get; set; }
        public int CryptoCurrencyId { get; set; }
        public decimal Amount { get; set; }
        public decimal InterestRate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ExpectedAmount { get; set; }
        public bool IsActive { get; set; }

    }

    public class InterestUpdateDto
    {
        public int CryptoCurrencyId { get; set; }
        public decimal InterestRate { get; set; }
    }

    public class InterestDto
    {
        public int Id { get; set; }
        public int CryptoCurrencyId { get; set; }
        public decimal InterestRate { get; set; }
    }
}
