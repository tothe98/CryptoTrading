using AutoMapper;
using CryptoTrading.DataContext;
using CryptoTrading.DataContext.Entities;
using CryptoTrading.DataContext.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace CryptoTrading.Services.Services
{
    public interface ISavingService
    {
        Task<ReturnSavingLockDto> LockSaving(CreateSavingLockDto savingLock);
        Task<List<SavingLockDto>> Get(int userid);
        Task<InterestUpdateDto> UpdateInterest(InterestUpdateDto dto);
        Task<List<InterestDto>> ListInterest();

        Task<String> Delete(int lockId);

    }

    public class SavingService : ISavingService
    {
        private readonly SQL _context;
        private readonly IMapper _mapper;

        public SavingService(SQL context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<String> Delete(int lockId)
        {
            var saving = await _context.SavingLocks.FirstOrDefaultAsync(s => s.Id == lockId);
            if (saving == null)
            {
                throw new Exception("Saving Lock not found");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == saving.UserId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == user.Id);
            if (wallet == null)
            {
                throw new Exception("Wallet not found");
            }

            var walletholding = await _context.WalletHoldings.FirstOrDefaultAsync(wh => wh.WalletId == wallet.Id && wh.CryptoCurrencyId == saving.CryptoCurrencyId);
            if (walletholding == null)
            {
                throw new Exception("Walletholding not found");
            }

            var tax = (saving.TotalPrice - saving.Amount) * (15 / 100);
            saving.IsActive = false;
            walletholding.Amount += saving.TotalPrice - tax;
            await _context.SaveChangesAsync();

            return $"Törlés után {saving.TotalPrice - tax} profit lett";


        }

        public async Task<List<SavingLockDto>> Get(int userid)
        {
            List<SavingLockDto> returnSavingLockDtos = new List<SavingLockDto>();
            var savings = await _context.SavingLocks.Where(s => s.UserId == userid).ToListAsync();
            foreach (var saving in savings)
            {
                returnSavingLockDtos.Add(new SavingLockDto()
                {
                    Amount = saving.Amount,
                    CryptoCurrencyId = saving.CryptoCurrencyId,
                    EndDate = saving.EndDate,
                    ExpectedAmount = saving.Amount * (decimal)Math.Pow((double)(1 + (saving.InterestRate / 100)), Math.Round((saving.EndDate - saving.StartDate).TotalDays)),
                    InterestRate = saving.InterestRate,
                    StartDate = saving.StartDate,
                    UserId = saving.UserId,
                    TotalAmount = saving.TotalPrice,
                    IsActive = saving.IsActive

                });

            }

            return returnSavingLockDtos;
        }

        public async Task<List<InterestDto>> ListInterest()
        {
            List<InterestDto> interestsList = new List<InterestDto>();
            var interests = await _context.CryptoInterestRates.ToListAsync();

            foreach (var interest in interests)
            {
                interestsList.Add(new InterestDto()
                {
                    CryptoCurrencyId = interest.CryptoCurrencyId,
                    Id = interest.Id,
                    InterestRate = interest.InterestRate
                });
            }

            return interestsList;
        }

        public async Task<ReturnSavingLockDto> LockSaving(CreateSavingLockDto savingLock)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == savingLock.UserId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == savingLock.UserId);
            if (wallet == null)
            {
                throw new Exception("Wallet not found");
            }

            var walletholding = await _context.WalletHoldings.FirstOrDefaultAsync(h => h.WalletId == wallet.Id && h.CryptoCurrencyId == savingLock.CryptoCurrencyId);

            if (walletholding == null)
            {
                throw new Exception("Crypto not found");
            }

            if (walletholding.Amount < savingLock.Amount || savingLock.Amount < 0)
            {
                throw new Exception("Not enough crypto");
            }

            if (savingLock.EndDate < DateTime.Now)
            {
                throw new Exception("Incorrect date");
            }


            var interestRate = await _context.CryptoInterestRates.FirstOrDefaultAsync(ci => ci.CryptoCurrencyId == savingLock.CryptoCurrencyId);
            if (interestRate == null)
            {
                throw new Exception("Interest Rate not found");
            }
            walletholding.Amount -= savingLock.Amount;
            await _context.SaveChangesAsync();


            var newsavinglock = await _context.SavingLocks.AddAsync(new SavingLock()
            {
                Amount = savingLock.Amount,
                UserId = savingLock.UserId,
                CryptoCurrencyId = savingLock.CryptoCurrencyId,
                StartDate = DateTime.Now,
                EndDate = savingLock.EndDate,
                InterestRate = interestRate.InterestRate,
                TotalPrice = savingLock.Amount,
            });

            await _context.SaveChangesAsync();

            var totalDay = Math.Round((savingLock.EndDate - DateTime.Now).TotalDays);



            return new ReturnSavingLockDto()
            {
                CryptoCurrencyId = newsavinglock.Entity.CryptoCurrencyId,
                Amount = newsavinglock.Entity.Amount,
                EndDate = newsavinglock.Entity.EndDate,
                StartDate = newsavinglock.Entity.StartDate,
                InterestRate = newsavinglock.Entity.InterestRate,
                UserId = newsavinglock.Entity.UserId,
                ExpectedAmount = savingLock.Amount * (decimal)Math.Pow((double)(1 + (interestRate.InterestRate / 100)), totalDay)
            };

        }

        public async Task<InterestUpdateDto> UpdateInterest(InterestUpdateDto dto)
        {
            var interest = await _context.CryptoInterestRates.FirstOrDefaultAsync(i => i.CryptoCurrencyId == dto.CryptoCurrencyId);
            if (interest == null)
            {
                throw new Exception("Interest not found");
            }

            interest.InterestRate = dto.InterestRate;
            await _context.SaveChangesAsync();

            return new InterestUpdateDto()
            {
                InterestRate = dto.InterestRate,
                CryptoCurrencyId = dto.CryptoCurrencyId
            };
        }
    }
}
